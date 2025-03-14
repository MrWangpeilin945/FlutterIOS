using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;
using Ryobi.Wellship.WebAPI.ExternalConnection.Enums;
using System;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;
/// <summary>
/// EC2004_受診を更新する
/// </summary>
public class ConsultUsecase : IConsultUsecase
{
    private List<ErrorObject> _errorObjects;
    private readonly IConsultRepository _consultRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly IPlaceRepository _placeRepository;
    private readonly IPlaceScheduleRepository _placeScheduleRepository;
    private readonly IExamineeRepository _examineeRepository;
    private readonly IThresholdRepository _thresholdRepository;
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="consultRepository">受診リポジトリ</param>
    /// <param name="teamRepository">班リポジトリ</param>
    /// <param name="placeRepository">会場リポジトリ</param>
    /// <param name="placeScheduleRepository">会場日程リポジトリ</param>
    /// <param name="examineeRepository">受診者リポジトリ</param>
    /// <param name="thresholdRepository">基準値パターンリポジトリ</param>
    /// <param name="timeProvider"></param>
    public ConsultUsecase(IConsultRepository consultRepository, ITeamRepository teamRepository, IPlaceRepository placeRepository,
                          IPlaceScheduleRepository placeScheduleRepository, IExamineeRepository examineeRepository,
                          IThresholdRepository thresholdRepository, TimeProvider timeProvider)
    {
        _consultRepository = consultRepository;
        _teamRepository = teamRepository;
        _placeRepository = placeRepository;
        _placeScheduleRepository = placeScheduleRepository;
        _examineeRepository = examineeRepository;
        _thresholdRepository = thresholdRepository;
        _errorObjects = new List<ErrorObject>();
        _timeProvider = timeProvider;
    }

    /// <summary>   
    /// EC2004_受診を更新する
    /// </summary>
    /// <param name="consults">更新する受診のリスト</param>
    /// <returns>エラーリスト</returns>
    public async Task<List<ErrorObject>> StoreConsultAsync(List<Consult> consults)
    {
        _errorObjects.Clear();

        var placeCodes = consults.Select(x => x.PlaceCode).ToList();
        var teamCodes = consults.Select(x => x.TeamCode).ToList();
        // 会場コードに紐づく会場IDを取得する
        var places = await _placeRepository.GetPlaceInfoAsync(placeCodes);
        // 班コードに紐づく班IDを取得する
        var teams = await _teamRepository.GetTeamInfoAsync(teamCodes);
        // 会場コード、班コード、健診日に紐づく会場日程情報を取得する
        var examDate = consults.Where(x => x.ActionType == ActionType.登録 && !string.IsNullOrWhiteSpace(x.ExamDate))
                               .Select(x => DateOnly.Parse(x.ExamDate)).ToList();
        var placeSchedules = await _placeScheduleRepository.GetPlaceScheduleInfoAsync(placeCodes, teamCodes, examDate);
        // 受診者コードに紐づく受診者IDを取得する
        var examinees = await _examineeRepository.GetExamineeInfoAsync(consults.Select(x => x.ExamineeCd).ToList());
        // 検査メニュー特記コードに紐づく情報を取得する
        var examMenuNodeCodes = await _consultRepository.GetExamMenuNodeCodeInfoAsync(
                                            consults.SelectMany(x => x.ConsultNotes.Select(cn => cn.Code)).ToList());
        // 基準値パターンコードに紐づく基準値パターンIDを取得する
        var thresholds = await _thresholdRepository.GetThresholdsByCodesAsync(
                                            consults.SelectMany(x => x.ConsultThresholds.Select(ct => ct.ThresholdCode)).ToList());
        // 検査項目明細CDに紐づく外部検査項目明細IDを取得する
        var detailCodes = consults.SelectMany(x => x.PreviousResults.Select(pr => pr.ExamItemDetailCd))
                                  .Concat(
                                      consults.SelectMany(x => x.ExamItemDetailOrders.Select(ei => ei.ExamItemDetailCd))
                                  ).ToList();
        var externalExamItemDetails = await _consultRepository.GetExternalExamItemDetailInfoAsync(detailCodes);
        // 連携キーに紐づく外部連携キーを取得する
        var externalConnectionCodes = await _consultRepository.GetExternalConnectionCodeAsync(consults.Select(x => x.ConnectionCode).ToList());
        // 受診番号に紐づく連携キーを取得する
        var consultExternalConnectionCodes = await _consultRepository.GetConsultExternalConnectionCodeAsync(consults.Select(x => x.ConsultNumber).ToList());

        // WARNING検証
        var warningConsult = new List<Consult>();
        // 必須項目の空値のチェック
        // キー重複チェック
        // 文字数チェック
        // 文字形式チェック

        // 受診番号が異なる連携キーで登録されている
        foreach (var warning in consults.Where(x => x.ActionType == ActionType.登録))
        {
            // 受診番号に紐づいている連携キーを取得する
            var connectionCode = consultExternalConnectionCodes.Where(x => x.ConsultNumber == warning.ConsultNumber)
                                                               .Select(x => x.ConnectionCode)
                                                               .FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(connectionCode) && connectionCode != warning.ConnectionCode)
            {
                warningConsult.Add(warning);
                _errorObjects.Add(new ErrorObject
                {
                    Code = "10002",
                    Message = $"指定されたConsultNumberが既に登録済みです。Code:{warning.ConsultNumber}",
                    InputNote = warning.InputNote
                });
            }
        }
        // 会場IDが取得できない
        foreach (var warning in consults.Where(x => x.ActionType == ActionType.登録)
                                        .Where(x => !places.Select(p => p.PlaceCode).Contains(x.PlaceCode)))
        {
            warningConsult.Add(warning);
            _errorObjects.Add(new ErrorObject
            {
                Code = "10001",
                Message = $"指定されたPlaceCodeがシステム上に存在しません。Code:{warning.PlaceCode}",
                InputNote = warning.InputNote
            });
        }
        // 班IDが取得できない
        foreach (var warning in consults.Where(x => x.ActionType == ActionType.登録)
                                        .Where(x => !teams.Select(p => p.TeamCode).Contains(x.TeamCode)))
        {
            warningConsult.Add(warning);
            _errorObjects.Add(new ErrorObject
            {
                Code = "10001",
                Message = $"指定されたTeamCodeがシステム上に存在しません。Code:{warning.TeamCode}",
                InputNote = warning.InputNote
            });
        }
        // 会場日程IDが取得できない
        foreach (var warning in consults.Where(x => x.ActionType == ActionType.登録)
                                        .Where(x => !placeSchedules.Any(ps => x.PlaceCode == ps.PlaceCode &&
                                                                              x.TeamCode == ps.TeamCode &&
                                                                              !string.IsNullOrWhiteSpace(x.ExamDate) &&
                                                                              DateOnly.Parse(x.ExamDate) == ps.ExamDate)))
        {
            warningConsult.Add(warning);
            _errorObjects.Add(new ErrorObject
            {
                Code = "10001",
                Message = $"指定されたPlaceScheduleがシステム上に存在しません。Code:PlaceCode:{warning.PlaceCode}/TeamCode:{warning.TeamCode}/ExamDate:{warning.ExamDate}",
                InputNote = warning.InputNote
            });
        }
        // 受診者IDが取得できない
        foreach (var warning in consults.Where(x => x.ActionType == ActionType.登録)
                                        .Where(x => !examinees.Select(e => e.ExamineeCode).Contains(x.ExamineeCd)))
        {
            warningConsult.Add(warning);
            _errorObjects.Add(new ErrorObject
            {
                Code = "10001",
                Message = $"指定されたExamineeCdがシステム上に存在しません。Code:{warning.ExamineeCd}",
                InputNote = warning.InputNote
            });
        }
        // 検査特記が存在しない
        foreach (var consult in consults.Where(x => x.ActionType == ActionType.登録))
        {
            foreach (var warning in consult.ConsultNotes.Where(x => !examMenuNodeCodes.Select(e => e.Code).Contains(x.Code)))
            {
                warningConsult.Add(consult);
                _errorObjects.Add(new ErrorObject
                {
                    Code = "10001",
                    Message = $"指定されたConsultNotes.Codeがシステム上に存在しません。Code:{warning.Code}",
                    InputNote = consult.InputNote
                });
            }
        }
        // 基準値パターンが取得できない
        foreach (var consult in consults.Where(x => x.ActionType == ActionType.登録))
        {
            foreach (var warning in consult.ConsultThresholds.Where(x => !thresholds.Select(t => t.ThresholdCode).Contains(x.ThresholdCode)))
            {
                warningConsult.Add(consult);
                _errorObjects.Add(new ErrorObject
                {
                    Code = "10001",
                    Message = $"指定されたConsultThresholds.ThresholdCodeがシステム上に存在しません。Code:{warning.ThresholdCode}",
                    InputNote = consult.InputNote
                });
            }
        }
        // 検査項目明細ID（PreviousResults）
        foreach (var consult in consults.Where(x => x.ActionType == ActionType.登録))
        {
            // 検査項目明細IDが取得できない
            foreach (var warning in consult.PreviousResults.Where(x => !externalExamItemDetails.Select(e => e.ExternalExamItemDetailCode).Contains(x.ExamItemDetailCd)))
            {
                warningConsult.Add(consult);
                _errorObjects.Add(new ErrorObject
                {
                    Code = "10001",
                    Message = $"指定されたPreviousResults.ExamItemDetailCdがシステム上に存在しません。Code:{warning.ExamItemDetailCd}",
                    InputNote = consult.InputNote
                });
            }
            // PKが重複するレコードが存在する
            // ExamItemDetailCd+ExamDateで重複する
            var duplicateExamItemDetailCds = consult.PreviousResults
                                                    .GroupBy(x => new { x.ExamItemDetailCd, x.ExamDate })
                                                    .Where(x => x.Count() > 1)
                                                    .Select(x => x);
            foreach (var warning in duplicateExamItemDetailCds)
            {
                warningConsult.Add(consult);
                _errorObjects.Add(new ErrorObject
                {
                    Code = "10003",
                    Message = $"キー項目が重複しています。Code:PreviousResults.ExamItemDetailCd:{warning.Key.ExamItemDetailCd}/PreviousResults.ExamDate:{warning.Key.ExamDate}",
                    InputNote = consult.InputNote
                });
            }
            // ExamItemDetailId+ExamDateで重複する
            var examItemDetail = from p in consult.PreviousResults
                                 join e in externalExamItemDetails
                                 on p.ExamItemDetailCd equals e.ExternalExamItemDetailCode
                                 select new
                                 {
                                     // external_exam_item_detailsのexam_item_detail_idを連結する
                                     ExamItemDetailId = e.ExamItemDetailId,
                                     ExamItemDetailCd = p.ExamItemDetailCd,
                                     ExamDate = p.ExamDate
                                 };
            var duplicateExamItemDetailIds = examItemDetail
                                                // ExamItemDetailCd+ExamDateの重複を取り除く
                                                .GroupBy(x => new { x.ExamItemDetailCd, x.ExamDate })
                                                .Select(x => x.First())
                                                // ExamItemDetailId+ExamDateの重複を取得する
                                                .GroupBy(x => new { x.ExamItemDetailId, x.ExamDate })
                                                .Where(x => x.Count() > 1)
                                                .SelectMany(x => x);
            foreach (var warning in duplicateExamItemDetailIds)
            {
                warningConsult.Add(consult);
                _errorObjects.Add(new ErrorObject
                {
                    Code = "10003",
                    Message = $"キー項目が重複しています。Code:PreviousResults.ExamItemDetailCd:{warning.ExamItemDetailCd}/PreviousResults.ExamDate:{warning.ExamDate}",
                    InputNote = consult.InputNote
                });
            }
        }
        // 検査項目明細ID（ExamItemDetailOrders）
        foreach (var consult in consults.Where(x => x.ActionType == ActionType.登録))
        {
            // 検査項目明細IDが取得できない
            foreach (var warning in consult.ExamItemDetailOrders.Where(x => !externalExamItemDetails.Select(e => e.ExternalExamItemDetailCode).Contains(x.ExamItemDetailCd)))
            {
                warningConsult.Add(consult);
                _errorObjects.Add(new ErrorObject
                {
                    Code = "10001",
                    Message = $"指定されたExamItemDetailOrders.ExamItemDetailCdがシステム上に存在しません。Code:{warning.ExamItemDetailCd}",
                    InputNote = consult.InputNote
                });
            }
            // PKが重複するレコードが存在する
            // ExamItemDetailCd+ExamDateで重複する
            var duplicateExamItemDetailCds = consult.ExamItemDetailOrders
                                                    .GroupBy(x => new { x.ExamItemDetailCd})
                                                    .Where(x => x.Count() > 1)
                                                    .Select(x => x);
            foreach (var warning in duplicateExamItemDetailCds)
            {
                warningConsult.Add(consult);
                _errorObjects.Add(new ErrorObject
                {
                    Code = "10003",
                    Message = $"キー項目が重複しています。Code:ExamItemDetailOrders.ExamItemDetailCd:{warning.Key.ExamItemDetailCd}",
                    InputNote = consult.InputNote
                });
            }
            // ExamItemDetailId+ExamDateで重複する
            var examItemDetail = from p in consult.ExamItemDetailOrders
                                 join e in externalExamItemDetails
                                 on p.ExamItemDetailCd equals e.ExternalExamItemDetailCode
                                 select new
                                 {
                                     // external_exam_item_detailsのexam_item_detail_idを連結する
                                     ExamItemDetailId = e.ExamItemDetailId,
                                     ExamItemDetailCd = p.ExamItemDetailCd,
                                 };
            var duplicateExamItemDetailIds = examItemDetail
                                                // ExamItemDetailCd+ExamDateの重複を取り除く
                                                .GroupBy(x => new { x.ExamItemDetailCd })
                                                .Select(x => x.First())
                                                // ExamItemDetailId+ExamDateの重複を取得する
                                                .GroupBy(x => new { x.ExamItemDetailId })
                                                .Where(x => x.Count() > 1)
                                                .SelectMany(x => x);
            foreach (var warning in duplicateExamItemDetailIds)
            {
                warningConsult.Add(consult);
                _errorObjects.Add(new ErrorObject
                {
                    Code = "10003",
                    Message = $"キー項目が重複しています。Code:ExamItemDetailOrders.ExamItemDetailCd:{warning.ExamItemDetailCd}",
                    InputNote = consult.InputNote
                });
            }
        }
        // 削除
        foreach (var warning in consults.Where(x => x.ActionType == ActionType.削除)
                                        .Where(x => !externalConnectionCodes.Select(x => x.ConnectionCode).Contains(x.ConnectionCode)))
        {
            warningConsult.Add(warning);
            _errorObjects.Add(new ErrorObject
            {
                Code = "10001",
                Message = $"指定されたConnectionCodeがシステム上に存在しません。Code:{warning.ConnectionCode}",
                InputNote = warning.InputNote
            });
        }
        // 受診を更新するリストを取得する
        var validConsults = consults.Where(x => !warningConsult.Select(w => w.ConnectionCode).Contains(x.ConnectionCode))
                                    .Select(x => new ConsultEntity
                                    {
                                        ActionType = x.ActionType,
                                        ConsultId = externalConnectionCodes.Where(ec => ec.ConnectionCode == x.ConnectionCode)
                                                                           .Select(ec => ec.ConsultId).FirstOrDefault(),
                                        ConsultNumber = x.ConsultNumber,
                                        PlaceScheduleId = placeSchedules.Where(ps => ps.PlaceCode == x.PlaceCode)
                                                                        .Where(ps => ps.TeamCode == x.TeamCode)
                                                                        .Where(ps => !string.IsNullOrWhiteSpace(x.ExamDate) &&
                                                                                     ps.ExamDate == DateOnly.Parse(x.ExamDate))
                                                                        .Select(ps => ps.PlaceScheduleId).FirstOrDefault(),
                                        Note = x.Note,
                                        ExamineeId = examinees.Where(e => e.ExamineeCode == x.ExamineeCd)
                                                              .Select(e => e.ExamineeId).FirstOrDefault(),
                                        ConnectionCode = x.ConnectionCode,
                                        SortNo = x.SortNo,
                                        ConsultNotes = x.ConsultNotes.Select(cn => new ConsultNoteEntity
                                        {
                                            Code = cn.Code,
                                            Note = cn.Note
                                        }).ToList(),
                                        ExamItemDetailOrders = x.ExamItemDetailOrders.Select(eo => new ExamItemDetailOrderEntity
                                        {
                                            ExamItemDetailId = externalExamItemDetails.Where(ed => ed.ExternalExamItemDetailCode == eo.ExamItemDetailCd)
                                                                                      .Select(ed => ed.ExamItemDetailId).FirstOrDefault(),
                                            ExamItemDetailCd = eo.ExamItemDetailCd
                                        }).ToList(),
                                        ConsultThresholds = x.ConsultThresholds.Select(ct => new ConsultThresholdEntity
                                        {
                                            ThresholdId = thresholds.Where(th => th.ThresholdCode == ct.ThresholdCode)
                                                                    .Select(th => th.ThresholdId).FirstOrDefault(),
                                            Priority = ct.Priority
                                        }).ToList(),
                                        PreviousResults = x.PreviousResults.Select(pr => new PreviousResultEntity
                                        {
                                            ExamDate = pr.ExamDate,
                                            ExamItemDetailId = externalExamItemDetails.Where(ed => ed.ExternalExamItemDetailCode == pr.ExamItemDetailCd)
                                                                                      .Select(ed => ed.ExamItemDetailId).FirstOrDefault(),
                                            Value = pr.Value
                                        }).ToList()
                                    }).ToList();
        // 受付を更新する
        await _consultRepository.UpsertConsultsAsync(validConsults, _timeProvider.GetUtcNow(), "ExternalConnection");
        return _errorObjects;
    }
}
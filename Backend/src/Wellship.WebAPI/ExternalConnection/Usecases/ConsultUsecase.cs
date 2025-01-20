using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;
using Npgsql.Internal;
using System.Globalization;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Usecases;
/// <summary>
/// EC2002_受付を更新する
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

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="consultRepository">受診リポジトリ</param>
    /// <param name="teamRepository">班リポジトリ</param>
    /// <param name="placeRepository">会場リポジトリ</param>
    /// <param name="placeScheduleRepository">会場日程リポジトリ</param>
    /// <param name="examineeRepository">受診者リポジトリ</param>
    /// <param name="thresholdRepository">基準値パターンリポジトリ</param>
    public ConsultUsecase(IConsultRepository consultRepository, ITeamRepository teamRepository, IPlaceRepository placeRepository,
                          IPlaceScheduleRepository placeScheduleRepository, IExamineeRepository examineeRepository, 
                          IThresholdRepository thresholdRepository)
    {
        _consultRepository = consultRepository;
        _teamRepository = teamRepository;
        _placeRepository = placeRepository;
        _placeScheduleRepository = placeScheduleRepository;
        _examineeRepository = examineeRepository;
        _thresholdRepository = thresholdRepository;
        _errorObjects = new List<ErrorObject>();
    }

    /// <summary>   
    /// EC2004_受診を更新する
    /// </summary>
    /// <param name="consults">更新する受診のリスト</param>
    /// <returns>エラーリスト</returns>
    public async Task<List<ErrorObject>> StoreConsultAsync(List<Consult> consults)
    {
        var placeCodes = consults.Select(x => x.PlaceCode).ToList();
        var teamCodes = consults.Select(x => x.TeamCode).ToList();
        // 会場コードに紐づく会場IDを取得する
        var places = await _placeRepository.GetPlaceInfoAsync(placeCodes);
        // 班コードに紐づく班IDを取得する
        var teams = await _teamRepository.GetTeamInfoAsync(teamCodes);
        // 会場コード、班コード、健診日に紐づく会場日程情報を取得する
        var placeSchedules = await _placeScheduleRepository.GetPlaceScheduleInfoAsync(placeCodes, teamCodes, consults.Select(x => x.ExamDate).ToList());
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
        var ExternalConnectionCodes = await _consultRepository.GetExternalConnectionCodeAsync(consults.Select(x => x.ConnectionCode).ToList());

        // WARNING検証
        var warningConsult = new List<Consult>();
        // 会場IDが取得できない
        foreach(var warning in consults.Where(x => !places.Select(p => p.PlaceCode).Contains(x.PlaceCode))
                                       .Where(x => !warningConsult.Select(c => c.ConnectionCode).ToList().Contains(x.ConnectionCode)))
        {
            warningConsult.Add(warning);
            _errorObjects.Add( new ErrorObject
            {
                Code = "10001",
                Message = $"指定されたPlaceCodeがシステム上に存在しません。Code:[{warning.PlaceCode}]",
                InputNote = warning.InputNote
            });
        }
        // 班IDが取得できない
        foreach(var warning in consults.Where(x => !teams.Select(p => p.TeamCode).Contains(x.TeamCode))
                                       .Where(x => !warningConsult.Select(w => w.ConnectionCode).Contains(x.ConnectionCode)))
        {
            warningConsult.Add(warning);
            _errorObjects.Add( new ErrorObject
            {
                Code = "10001",
                Message = $"指定されたTeamCodeがシステム上に存在しません。Code:[{warning.TeamCode}]",
                InputNote = warning.InputNote
            });
        }
        // 会場日程IDが取得できない
        foreach(var warning in consults.Where(x => !placeSchedules.Any(ps => x.PlaceCode == ps.PlaceCode &&
                                                                             x.TeamCode == ps.TeamCode &&
                                                                             x.ExamDate == DateOnly.FromDateTime(ps.ExamDate)))
                                       .Where(x => !warningConsult.Select(w => w.ConnectionCode).Contains(x.ConnectionCode)))
        {
            warningConsult.Add(warning);
            _errorObjects.Add( new ErrorObject
            {
                Code = "10001",
                Message = $"指定されたPlaceScheduleがシステム上に存在しません。Code:PlaceCode:[{warning.PlaceCode}]/TeamCode:[{warning.TeamCode}]/ExamDate:[{warning.ExamDate}]",
                InputNote = warning.InputNote
            });
        }
        // 受診者IDが取得できない
        foreach(var warning in consults.Where(x => !examinees.Select(e => e.ExamineeCode).Contains(x.ExamineeCd))
                                        .Where(x => !warningConsult.Select(w => w.ConnectionCode).Contains(x.ConnectionCode)))
        {
            warningConsult.Add(warning);
            _errorObjects.Add( new ErrorObject
            {
                Code = "10001",
                Message = $"指定されたExamineeCdがシステム上に存在しません。Code:[{warning.ExamineeCd}]",
                InputNote = warning.InputNote
            });
        }
        // 検査特記が存在しない
        foreach(var consult in consults.Where(x => !warningConsult.Select(w => w.ConnectionCode).Contains(x.ConnectionCode)))
        {
            foreach(var warning in consult.ConsultNotes.Where(x => !examMenuNodeCodes.Select(e => e.Code).Contains(x.Code)))
            {
                warningConsult.Add(consult);
                _errorObjects.Add( new ErrorObject
                {
                    Code = "10001",
                    Message = $"指定されたConsultNotes.Codeがシステム上に存在しません。Code:[{warning.Code}]",
                    InputNote = consult.InputNote
                });
                break;
            }
        }
        // 基準値パターンが取得できない
        foreach(var consult in consults.Where(x => !warningConsult.Select(w => w.ConnectionCode).Contains(x.ConnectionCode)))
        {
            foreach(var warning in consult.ConsultThresholds.Where(x=> !thresholds.Select(t => t.ThresholdCode).Contains(x.ThresholdCode)))
            {
                warningConsult.Add(consult);
                _errorObjects.Add( new ErrorObject
                {
                    Code = "10001",
                    Message = $"指定されたConsultThresholds.ThresholdCodeがシステム上に存在しません。Code:[{warning.ThresholdCode}]",
                    InputNote = consult.InputNote
                });
                break;
            }
        }
        // 検査項目明細ID（PreviousResults）
        foreach(var consult in consults.Where(x => !warningConsult.Select(w => w.ConnectionCode).Contains(x.ConnectionCode)))
        {
            // 検査項目明細IDが取得できない
            foreach(var warning in consult.PreviousResults.Where(x=> !externalExamItemDetails.Select(e => e.ExternalExamItemDetailCode).Contains(x.ExamItemDetailCd)))
            {
                warningConsult.Add(consult);
                _errorObjects.Add( new ErrorObject
                {
                    Code = "10001",
                    Message = $"指定されたPreviousResults.ExamItemDetailCdがシステム上に存在しません。Code:[{warning.ExamItemDetailCd}]",
                    InputNote = consult.InputNote
                });
                break;
            }
            // PKが重複するレコードが存在する
            foreach(var previousResult in consult.PreviousResults)
            {
                if (externalExamItemDetails.Count(x => x.ExternalExamItemDetailCode == previousResult.ExamItemDetailCd) > 1)
                {
                    warningConsult.Add(consult);
                    _errorObjects.Add( new ErrorObject
                    {
                        Code = "10003",
                        Message = $"キー項目が重複しています。Code:ExamItemDetailCd:[{previousResult.ExamItemDetailCd}]",
                        InputNote = consult.InputNote
                    });
                    break;
                }
            }
        }
        // 検査項目明細ID（ExamItemDetailOrders）
        foreach(var consult in consults.Where(x => !warningConsult.Select(w => w.ConnectionCode).Contains(x.ConnectionCode)))
        {
            // 検査項目明細IDが取得できない
            foreach(var warning in consult.ExamItemDetailOrders.Where(x=> !externalExamItemDetails.Select(e => e.ExternalExamItemDetailCode).Contains(x.ExamItemDetailCd)))
            {
                warningConsult.Add(consult);
                _errorObjects.Add( new ErrorObject
                {
                    Code = "10001",
                    Message = $"指定されたExamItemDetailOrders.ExamItemDetailCdがシステム上に存在しません。Code:[{warning.ExamItemDetailCd}]",
                    InputNote = consult.InputNote
                });
                break;
            }
            // PKが重複するレコードが存在する
            foreach(var examItemDetailOrder in consult.ExamItemDetailOrders)
            {
                if (externalExamItemDetails.Count(x => x.ExternalExamItemDetailCode == examItemDetailOrder.ExamItemDetailCd) > 1)
                {
                    warningConsult.Add(consult);
                    _errorObjects.Add( new ErrorObject
                    {
                        Code = "10003",
                        Message = $"キー項目が重複しています。Code:ExamItemDetailCd:[{examItemDetailOrder.ExamItemDetailCd}]",
                        InputNote = consult.InputNote
                    });
                    break;
                }
            }
        }
        // 削除
        foreach(var warning in consults.Where(x => x.ActionType == Wellship.ExternalConnection.Enums.ActionType.削除)
                                       .Where(x => !ExternalConnectionCodes.Select(x => x.ConnectionCode).Contains(x.ConnectionCode)) 
                                       .Where(x => !warningConsult.Select(w => w.ConnectionCode).Contains(x.ConnectionCode)))
        {
            warningConsult.Add(warning);
            _errorObjects.Add( new ErrorObject
            {
                Code = "10001",
                Message = $"指定されたConnectionCodeがシステム上に存在しません。Code:[{warning.ConnectionCode}]",
                InputNote = warning.InputNote
            });
        }
        // 受診を更新するリストを取得する
        var validConsults = consults.Where(x => !warningConsult.Select(w => w.ConnectionCode).Contains(x.ConnectionCode))
                                    .Select(x => new ConsultEntity
                                    {
                                        ActionType = x.ActionType
                                        , ConsultId = ExternalConnectionCodes.Where(ec => ec.ConnectionCode == x.ConnectionCode)
                                                                             .Select(ec => ec.ConsultId).FirstOrDefault()
                                        , ConsultNumber = x.ConsultNumber
                                        , PlaceScheduleId = placeSchedules.Where(ps => ps.PlaceCode == x.PlaceCode)
                                                                          .Where(ps => ps.TeamCode == x.TeamCode)
                                                                          .Where(ps => DateOnly.FromDateTime(ps.ExamDate) == x.ExamDate)
                                                                          .Select(ps => ps.PlaceScheduleId).FirstOrDefault()
                                        , Note = x.Note
                                        , ExamineeId = examinees.Where(e => e.ExamineeCode == x.ExamineeCd)
                                                                .Select(e => e.ExamineeId).FirstOrDefault()
                                        , ConnectionCode = x.ConnectionCode
                                        , SortNo = x.SortNo
                                        , ConsultNotes = x.ConsultNotes.Select(cn => new ConsultNoteEntity
                                        { 
                                            Code = cn.Code
                                            , Note = cn.Note 
                                        }).ToList()
                                        , ExamItemDetailOrders = x.ExamItemDetailOrders.Select(eo => new ExamItemDetailOrderEntity
                                        {
                                            ExamItemDetailId = externalExamItemDetails.Where(ed => ed.ExternalExamItemDetailCode == eo.ExamItemDetailCd)
                                                                                      .Select(ed => ed.ExamItemDetailId).FirstOrDefault()
                                            , ExamItemDetailCd = eo.ExamItemDetailCd
                                        }).ToList()
                                        , ConsultThresholds =x.ConsultThresholds.Select(ct => new ConsultThresholdEntity
                                        {
                                            ThresholdId = thresholds.Where(th => th.ThresholdCode == ct.ThresholdCode)
                                                                    .Select(th => th.ThresholdId).FirstOrDefault()
                                            , Priority = ct.Priority
                                        }).ToList()
                                        , PreviousResults = x.PreviousResults.Select(pr => new PreviousResultEntity
                                        {
                                            ExamDate = pr.ExamDate
                                            , ExamItemDetailId = externalExamItemDetails.Where(ed => ed.ExternalExamItemDetailCode == pr.ExamItemDetailCd)
                                                                                        .Select(ed => ed.ExamItemDetailId).FirstOrDefault()
                                            , Value = pr.Value
                                        }).ToList()
                                    }).ToList();        
        // 受付を更新する
        await _consultRepository.UpsertConsultsAsync(validConsults, DateTime.Now, "ExternalConnection");                        
        return _errorObjects;
    }
}
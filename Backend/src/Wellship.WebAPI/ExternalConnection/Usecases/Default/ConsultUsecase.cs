using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;
using Ryobi.Wellship.WebAPI.ExternalConnection.Enums;
using Ryobi.Wellship.WebAPI.ExternalConnection.Utilities;
using System.Globalization;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;
/// <summary>
/// EC2004_受診を更新する
/// </summary>
public class ConsultUsecase : IConsultUsecase
{
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
        _timeProvider = timeProvider;
    }

    /// <summary>   
    /// EC2004_受診を更新する
    /// </summary>
    /// <param name="consults">更新する受診のリスト</param>
    /// <returns>エラーリスト</returns>
    public async Task<List<ErrorObject>> StoreConsultAsync(List<Consult> consults)
    {
        List<ErrorObject> errorObjects = new List<ErrorObject>();

        var placeCodes = consults.Select(x => x.PlaceCode).Distinct().ToList();
        var teamCodes = consults.Select(x => x.TeamCode).Distinct().ToList();
        // 会場コードに紐づく会場IDを取得する
        var places = await _placeRepository.GetPlaceInfoAsync(placeCodes);
        // 班コードに紐づく班IDを取得する
        var teams = await _teamRepository.GetTeamInfoAsync(teamCodes);
        // 会場コード、班コード、健診日に紐づく会場日程情報を取得する
        var examDate = consults.Where(x => x.ActionType == ActionType.登録 &&
                                      !string.IsNullOrWhiteSpace(x.ExamDate))
                               .Select(x => DateOnly.Parse(x.ExamDate, CultureInfo.CurrentCulture))
                               .Distinct()
                               .ToList();
        var placeSchedules = await _placeScheduleRepository.GetPlaceScheduleInfoAsync(placeCodes, teamCodes, examDate);
        // 受診者コードに紐づく受診者IDを取得する
        var examinees = await _examineeRepository.GetExamineeInfoAsync(consults.Select(x => x.ExamineeCode).Distinct().ToList());
        // 検査メニュー特記コードに紐づく情報を取得する
        var examMenuNodeCodes = await _consultRepository.GetExamMenuNodeCodeInfoAsync(
                                            consults.SelectMany(x => x.ConsultNotes.Select(cn => cn.Code)).Distinct().ToList());
        // 基準値パターンコードに紐づく基準値パターンIDを取得する
        var thresholds = await _thresholdRepository.GetThresholdsByCodesAsync(
                                            consults.SelectMany(x => x.ConsultThresholds.Select(ct => ct.ThresholdCode)).Distinct().ToList());
        // 検査項目明細CDに紐づく外部検査項目明細IDを取得する
        var detailCodes = consults.SelectMany(x => x.PreviousResults.Select(pr => pr.ExamItemDetailCode))
                                  .Concat(
                                      consults.SelectMany(x => x.ExamItemDetailOrders.Select(ei => ei.ExamItemDetailCode))
                                  ).ToList();
        var externalExamItemDetails = await _consultRepository.GetExternalExamItemDetailInfoAsync(detailCodes);
        // 連携キーに紐づく外部連携キーを取得する
        var externalConnectionCodes = await _consultRepository.GetExternalConnectionCodeAsync(consults.Select(x => x.ConnectionCode).Distinct().ToList());
        // 受診番号に紐づく連携キーを取得する
        var consultExternalConnectionCodes = await _consultRepository.GetConsultExternalConnectionCodeAsync(consults.Select(x => x.ConsultNumber).Distinct().ToList());

        // WARNING検証
        var warningConsults = new List<Consult>();
        // 連携モードが登録のみを抽出
        var registeConsults = consults.Where(x => x.ActionType == ActionType.登録);

        // 必須項目の空値のチェック
        // 連携キー（共通）
        var spaceCheckCommonProperties = new[]
        {
            "ConnectionCode"     // 連携キー
        };
        AddWarningConsults(warningConsults, ValidationChecker.SpaceCheckProperties(consults, spaceCheckCommonProperties, errorObjects));
        var spaceCheckProperties = new[]
        {
            "PlaceCode",         // 会場コード
            "TeamCode",          // 班コード
            "ConsultNumber",     // 受診番号
            "ExamineeCode",      // 受診者コード
            "Age"                // 年齢
        };
        AddWarningConsults(warningConsults, ValidationChecker.SpaceCheckProperties(registeConsults, spaceCheckProperties, errorObjects));
        var spaceCheckChildrenProperties = new List<(string ParentProperty, string ChildProperty)>
        {
            ("PreviousResults",      "ExamItemDetailCode"),     // 過去検査結果->検査項目明細CD
            ("PreviousResults",      "Value"),                  // 過去検査結果->結果値
            ("ConsultThresholds",    "ThresholdCode"),          // 基準値判定->基準値判定コード
            ("ConsultNotes",         "Code"),                   // 受診特記->検査特記コード
            ("ExamItemDetailOrders", "ExamItemDetailCode")      // 検査項目明細依頼->検査項目明細CD
        };
        AddWarningConsults(warningConsults, ValidationChecker.SpaceCheckChildrenProperties(registeConsults, spaceCheckChildrenProperties, errorObjects));

        // キー重複チェック
        var duplicateCheckProperties = new[]
        {
            "SortNo"            // 処理順
        };
        AddWarningConsults(warningConsults, ValidationChecker.DuplicateCheckProperties(registeConsults, duplicateCheckProperties, errorObjects));
        var duplicateCheckChildProperties = new List<(string ParentProperty, string ChildProperty)>
        {
            ("ConsultThresholds",   "ThresholdCode"),       // 基準値判定->基準値判定コード
            ("ConsultThresholds",   "Priority"),            // 基準値判定->優先
            ("ConsultNotes",        "Code"),                // 受診特記->検査特記コード
            ("ExamItemDetailOrders","ExamItemDetailCode")   // 検査項目明細依頼->検査項目明細CD
        };
        AddWarningConsults(warningConsults, ValidationChecker.DuplicateCheckChildrenProperties(registeConsults, duplicateCheckChildProperties, errorObjects));

        // 文字数のチェック
        var stringLengthCheckProperties = new[]
        {
            "ConsultNumber",            // 受診番号　50桁以下
            "Age"                       // 年齢　　　 7桁　
        };
        var stringLengths = new[] { 50, 7 };
        var compars = new[] { -1, 0 };
        AddWarningConsults(warningConsults, ValidationChecker.StringLengthCheckProperties(registeConsults, stringLengthCheckProperties, stringLengths, compars, errorObjects));

        // 文字形式のチェック
        var stringPatternCheckProperties = new[]
        {
            "ConsultNumber",            // 受診番号
            "Age"                       // 年齢
        };
        var patterns = new[] { @"^[a-zA-Z0-9]+$", @"^[0-9]{3}(0[0-9]|1[01])([012][0-9]|30)$" };
        AddWarningConsults(warningConsults, ValidationChecker.StringPatternCheckProperties(registeConsults, stringPatternCheckProperties, patterns, errorObjects));

        // 受診番号が異なる連携キーで登録されている
        foreach (var warning in registeConsults)
        {
            // 受診番号に紐づいている連携キーを取得する
            var connectionCode = consultExternalConnectionCodes.Where(x => x.ConsultNumber == warning.ConsultNumber)
                                                               .Select(x => x.ConnectionCode)
                                                               .FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(connectionCode) && connectionCode != warning.ConnectionCode)
            {
                warningConsults.Add(warning);
                AddErrorObjects(errorObjects, "10002", $"指定されたConsultNumberが既に登録済みです。Code:{warning.ConsultNumber}", warning.InputNote);
            }
        }
        // 会場IDが取得できない
        var notPlaces = registeConsults.Where(x => !places.Select(p => p.PlaceCode).Contains(x.PlaceCode));
        ValidateCodes(notPlaces, warningConsults, errorObjects,
                        (warning) => $"指定されたPlaceCodeがシステム上に存在しません。Code:{warning.PlaceCode}");
        // 班IDが取得できない
        var notTeams = registeConsults.Where(x => !teams.Select(p => p.TeamCode).Contains(x.TeamCode));
        ValidateCodes(notTeams, warningConsults, errorObjects,
                        (warning) => $"指定されたTeamCodeがシステム上に存在しません。Code:{warning.TeamCode}");
        // 会場日程IDが取得できない
        var notPlaceSchedules = registeConsults.Where(x => !placeSchedules.Exists(ps => x.PlaceCode == ps.PlaceCode &&
                                                        x.TeamCode == ps.TeamCode &&
                                                        !string.IsNullOrWhiteSpace(x.ExamDate) &&
                                                        DateOnly.Parse(x.ExamDate, CultureInfo.CurrentCulture) == ps.ExamDate));
        ValidateCodes(notPlaceSchedules, warningConsults, errorObjects,
                        (warning) => $"指定されたPlaceScheduleがシステム上に存在しません。Code:PlaceCode:{warning.PlaceCode}/TeamCode:{warning.TeamCode}/ExamDate:{warning.ExamDate}");

        // 受診者IDが取得できない
        var notExaminees = registeConsults.Where(x => !examinees.Select(e => e.ExamineeCode).Contains(x.ExamineeCode));
        ValidateCodes(notExaminees, warningConsults, errorObjects,
                        (warning) => $"指定されたExamineeCodeがシステム上に存在しません。Code:{warning.ExamineeCode}");
        // 検査特記が存在しない
        ValidateChildCodes(registeConsults, x => x.ConsultNotes, x => x.Code, examMenuNodeCodes?.Select(e => e.Code).ToList() ?? new List<string>(),
                            warningConsults, errorObjects, (warning) => $"指定されたConsultNotes.Codeがシステム上に存在しません。Code:{warning.Code}");
        // 基準値パターンが取得できない
        ValidateChildCodes(registeConsults, x => x.ConsultThresholds, x => x.ThresholdCode, thresholds?.Select(e => e.ThresholdCode).ToList() ?? new List<string>(),
                            warningConsults, errorObjects, (warning) => $"指定されたConsultThresholds.ThresholdCodeがシステム上に存在しません。Code:{warning.ThresholdCode}");
        // 検査項目明細IDが取得できない
        ValidateChildCodes(registeConsults, x => x.PreviousResults, x => x.ExamItemDetailCode, externalExamItemDetails?.Select(e => e.ExternalExamItemDetailCode).ToList() ?? new List<string>(),
                            warningConsults, errorObjects, (warning) => $"指定されたPreviousResults.ExamItemDetailCodeがシステム上に存在しません。Code:{warning.ExamItemDetailCode}");
        // 検査項目明細ID（PreviousResults）
        foreach (var consult in registeConsults)
        {
            // PKが重複するレコードが存在する
            // ExamItemDetailCode+ExamDateで重複する
            var duplicateExamItemDetailCds = consult.PreviousResults
                                                    .GroupBy(x => new { x.ExamItemDetailCode, x.ExamDate })
                                                    .Where(x => x.Count() > 1)
                                                    .Select(x => x);
            foreach (var key in duplicateExamItemDetailCds.Select(warning => warning.Key))
            {
                warningConsults.Add(consult);
                AddErrorObjects(errorObjects, "10003",
                    $"キー項目が重複しています。PreviousResults.ExamItemDetailCode:{key.ExamItemDetailCode}/PreviousResults.ExamDate:{key.ExamDate.ToString("yyyy/MM/dd")}",
                    consult.InputNote);
            }
            // ExamItemDetailId+ExamDateで重複する
            var examItemDetail = from p in consult.PreviousResults
                                 join e in externalExamItemDetails ?? new List<ExternalExamItemDetailEntity>()
                                 on p.ExamItemDetailCode equals e.ExternalExamItemDetailCode
                                 select new
                                 {
                                     // external_exam_item_detailsのexam_item_detail_idを連結する
                                     ExamItemDetailId = e.ExamItemDetailId,
                                     ExamItemDetailCode = p.ExamItemDetailCode,
                                     ExamDate = p.ExamDate
                                 };
            var duplicateExamItemDetailIds = examItemDetail
                                                // ExamItemDetailCode+ExamDateの重複を取り除く
                                                .GroupBy(x => new { x.ExamItemDetailCode, x.ExamDate })
                                                .Select(x => x.First())
                                                // ExamItemDetailId+ExamDateの重複を取得する
                                                .GroupBy(x => new { x.ExamItemDetailId, x.ExamDate })
                                                .Where(x => x.Count() > 1)
                                                .SelectMany(x => x);
            foreach (var warning in duplicateExamItemDetailIds)
            {
                warningConsults.Add(consult);
                AddErrorObjects(errorObjects, "10003",
                    $"キー項目が重複しています。PreviousResults.ExamItemDetailCode:{warning.ExamItemDetailCode}/PreviousResults.ExamDate:{warning.ExamDate.ToString("yyyy/MM/dd")}",
                    consult.InputNote);
            }
        }
        // 検査項目明細ID（ExamItemDetailOrders）
        ValidateChildCodes(registeConsults, x => x.ExamItemDetailOrders, x => x.ExamItemDetailCode, externalExamItemDetails?.Select(e => e.ExternalExamItemDetailCode).ToList() ?? new List<string>(),
                            warningConsults, errorObjects, (warning) => $"指定されたExamItemDetailOrders.ExamItemDetailCodeがシステム上に存在しません。Code:{warning.ExamItemDetailCode}");
        // 削除
        var notExternalConnections = consults.Where(x => x.ActionType == ActionType.削除)
                                             .Where(x => !externalConnectionCodes.Select(x => x.ConnectionCode).Contains(x.ConnectionCode));
        ValidateCodes(notExternalConnections, warningConsults, errorObjects,
                     (warning) => $"指定されたConnectionCodeがシステム上に存在しません。Code:{warning.ConnectionCode}");

        // 受診を更新するエンティティを作成する
        var validConsults = consults.Except(warningConsults)
                                    .Select(x => new ConsultEntity
                                    {
                                        ActionType = x.ActionType,
                                        ConsultId = externalConnectionCodes.Where(ec => ec.ConnectionCode == x.ConnectionCode)
                                                                           .Select(ec => ec.ConsultId).FirstOrDefault(),
                                        ConsultNumber = x.ConsultNumber,
                                        PlaceScheduleId = placeSchedules.Where(ps => ps.PlaceCode == x.PlaceCode)
                                                                        .Where(ps => ps.TeamCode == x.TeamCode)
                                                                        .Where(ps => !string.IsNullOrWhiteSpace(x.ExamDate) &&
                                                                                     ps.ExamDate == DateOnly.Parse(x.ExamDate, CultureInfo.CurrentCulture))
                                                                        .Select(ps => ps.PlaceScheduleId).FirstOrDefault(),
                                        Note = x.Note,
                                        ExamineeId = examinees.Where(e => e.ExamineeCode == x.ExamineeCode)
                                                              .Select(e => e.ExamineeId).FirstOrDefault(),
                                        Age = x.Age,
                                        ConnectionCode = x.ConnectionCode,
                                        SortNo = x.SortNo,
                                        ConsultNotes = x.ConsultNotes.Select(cn => new ConsultNoteEntity
                                        {
                                            Code = cn.Code,
                                            Note = cn.Note
                                        }).ToList(),
                                        ExamItemDetailOrders = x.ExamItemDetailOrders.Select(eo => new ExamItemDetailOrderEntity
                                        {
                                            ExamItemDetailId = (externalExamItemDetails ?? new List<ExternalExamItemDetailEntity>())
                                                                    .Where(ed => ed.ExternalExamItemDetailCode == eo.ExamItemDetailCode)
                                                                    .Select(ed => ed.ExamItemDetailId).FirstOrDefault(),
                                            ExamItemDetailCode = eo.ExamItemDetailCode,
                                            Note = eo.Note
                                        }).ToList(),
                                        ConsultThresholds = x.ConsultThresholds.Select(ct => new ConsultThresholdEntity
                                        {
                                            ThresholdId = (thresholds ?? new List<ThresholdEntity>())
                                                                .Where(th => th.ThresholdCode == ct.ThresholdCode)
                                                                .Select(th => th.ThresholdId).FirstOrDefault(),
                                            Priority = ct.Priority
                                        }).ToList(),
                                        PreviousResults = x.PreviousResults.Select(pr => new PreviousResultEntity
                                        {
                                            ExamDate = pr.ExamDate,
                                            ExamItemDetailId = (externalExamItemDetails ?? new List<ExternalExamItemDetailEntity>())
                                                                    .Where(ed => ed.ExternalExamItemDetailCode == pr.ExamItemDetailCode)
                                                                    .Select(ed => ed.ExamItemDetailId).FirstOrDefault(),
                                            Value = pr.Value
                                        }).ToList()
                                    }).ToList();
        // 受付を更新する
        await _consultRepository.UpsertConsultsAsync(validConsults, _timeProvider.GetUtcNow(), "ExternalConnection");
        return errorObjects;
    }
    /// <summary>
    /// エラーのオブジェクトをconsultにキャストしてワーニングリストに追加する
    /// </summary>
    /// <param name="warningConsults"></param>
    /// <param name="warnings"></param>    
    private static void AddWarningConsults(List<Consult> warningConsults, IEnumerable<object> warnings)
    {
        foreach (var warning in warnings)
        {
            warningConsults.Add((Consult)warning);
        }
    }
    /// <summary>
    /// ErrorObjectにWARNING検証のメッセージを追加する
    /// </summary>
    /// <param name="errorObjects"></param>
    /// <param name="code"></param>
    /// <param name="message"></param>
    /// <param name="inputNote"></param>
    private static void AddErrorObjects(List<ErrorObject> errorObjects, string code, string message, string inputNote)
    {
        errorObjects.Add(new ErrorObject
        {
            Code = code,
            Message = message,
            InputNote = inputNote
        });
    }
    /// <summary>
    /// 存在しないデータを含むconsultをエラーオブジェクトに追加する
    /// </summary>
    /// <param name="consults"></param>
    /// <param name="warningConsults"></param>
    /// <param name="errorObjects"></param>
    /// <param name="messageGenerator"></param>
    private static void ValidateCodes(IEnumerable<Consult> consults, List<Consult> warningConsults,
                                      List<ErrorObject> errorObjects, Func<Consult, string> messageGenerator)
    {
        foreach (var warning in consults)
        {
            warningConsults.Add(warning);
            AddErrorObjects(errorObjects, "10001", messageGenerator(warning), warning.InputNote);
        }
    }
    /// <summary>
    /// 子プロパティの存在しないデータを含むconsultをエラーオブジェクトに追加する
    /// </summary>
    /// <typeparam name="TChild"></typeparam>
    /// <param name="consults"></param>
    /// <param name="childSelector"></param>
    /// <param name="codeSelector"></param>
    /// <param name="validCodes"></param>
    /// <param name="warningConsults"></param>
    /// <param name="errorObjects"></param>
    /// <param name="messageGenerator"></param>
    private static void ValidateChildCodes<TChild>(IEnumerable<Consult> consults, Func<Consult, IEnumerable<TChild>> childSelector,
                                                   Func<TChild, string> codeSelector, List<string> validCodes, List<Consult> warningConsults,
                                                   List<ErrorObject> errorObjects, Func<TChild, string> messageGenerator)
    {
        foreach (var consult in consults)
        {
            foreach (var warning in childSelector(consult).Where(x => !validCodes.Contains(codeSelector(x))))
            {
                warningConsults.Add(consult);
                AddErrorObjects(errorObjects, "10001", messageGenerator(warning), consult.InputNote);
            }
        }
    }
}
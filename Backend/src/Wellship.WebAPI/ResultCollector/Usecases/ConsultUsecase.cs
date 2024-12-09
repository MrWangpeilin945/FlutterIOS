using Ryobi.Wellship.APIModels.Requests;
using Ryobi.Wellship.APIModels.Responses;
using Ryobi.Wellship.Core.Exceptions;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

/// <summary>
/// 受診ユースケース
/// </summary>
public class ConsultUsecase : IConsultUsecase
{
    private readonly IConsultRepository _consultRepository;
    private readonly IExamineeRepository _examineeRepository;
    private readonly IExamItemRepository _examItemRepository;
    private readonly IPlaceScheduleRepository _placeScheduleRepository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="consultRepository">受診リポジトリ</param>
    /// <param name="examineeRepository">受診者リポジトリ</param>
    /// <param name="examItemRepository">検査項目リポジトリ</param>
    /// <param name="placeScheduleRepository">会場日程リポジトリ</param>
    public ConsultUsecase(IConsultRepository consultRepository, IExamineeRepository examineeRepository, 
                          IExamItemRepository examItemRepository, IPlaceScheduleRepository placeScheduleRepository)
    {
        _consultRepository = consultRepository;
        _examineeRepository = examineeRepository;
        _examItemRepository = examItemRepository;
        _placeScheduleRepository = placeScheduleRepository;
    }

    /// <summary>
    /// 受診番号の受診が存在するか確認する
    /// </summary>
    /// <param name="consultNumberRequest">受診番号リクエスト</param>
    /// <returns>受診が存在するか</returns>
    public async Task VerifyConsultNumberAsync(ConsultNumberRequest consultNumberRequest)
    {
        var consultNumber = consultNumberRequest.ConsultNumber;
        var consultExists = await _consultRepository.ConsultExistsAsync(consultNumber);

        if (!consultExists)
        {
            throw new ConsultNumberNotFoundException("受診番号が存在しません。");
        }
    }

    /// <summary>
    /// 未受診の検査メニューを取得する
    /// </summary>
    public async Task<UnexaminedMenuList> GetUnexaminedMenusAsync(string consultNumber)
    {
        // 受診単位に紐づく未受診の検査項目を取得する
        // 検査項目明細単位の依頼に対して、検査結果あるいは検査中止のレコードが存在すれば受診済みとする
        // 未受診の検査項目明細が1つ以上存在する検査メニューを「未受診の検査メニュー」として返す

        var consult = await _consultRepository.GetConsultAsync(consultNumber);
        var examinee = await _examineeRepository.GetExamineeAsync(consult.ExamineeId);

        // 未受診の検査メニューを取得する
        var unexaminedConsults = await _consultRepository.GetUnexaminedConsultsAsync([consult.ConsultNumber]);

        // 未受診の検査メニューがない受診の場合は、空リストを返す
        if (!unexaminedConsults.Any())
        {
            return new UnexaminedMenuList()
            {
                ConsultId = consult.ConsultId,
                ExamineeId = examinee.ExamineeId,
                ExamineeName = examinee.Name,
                UnexaminedMenus = []
            };
        }

        // 未受診項目がある受診の場合は、検査メニュー単位の未受診リストを返す
        var unexaminedConsult = unexaminedConsults.Single();
        return new UnexaminedMenuList()
        {
            ConsultId = unexaminedConsult.ConsultId,
            ExamineeId = examinee.ExamineeId,
            ExamineeName = examinee.Name,
            UnexaminedMenus = unexaminedConsult.UnexaminedExamMenus.Select(x => new ExamMenu()
            {
                ExamMenuId = x.ExamMenuId,
                ExamMenuName = x.ExamMenuName
            }).ToArray()
        };
    }

    /// <summary>
    /// 簡易な受診者情報を取得する
    /// </summary>
    public void GetSimpleExaminee()
    {

    }

    /// <summary>
    /// 検査内容を取得する
    /// </summary>
    public void GetExamItemsExaminee()
    {

    }

    /// <summary>
    /// 検査の実施有無と中止理由を登録する
    /// </summary>
    public async Task RegisterExecutionsAsync(string consultNumber, ExecutionsRequest request)
    {
        // NOTE: 中止理由の登録ルール
        // 前提：リクエストは検査項目単位、DBは検査項目明細単位
        // 
        // [1] isPerforming（検査実施する）：true & 中止理由：null
        //   - a. 中止レコードがある => 中止レコードを削除する
        //   - b. 中止レコードがない => 処理しない
        //
        // [2] isPerforming（検査実施する）：false & 中止理由：あり
        //   - a. 中止レコードがある => 中止レコードを更新する
        //   - b. 中止レコードがない => 中止レコードを挿入する

        var consult = await _consultRepository.GetConsultAsync(consultNumber);
        var examCancels = await _consultRepository.GetExamCancelsAsync(consult.ConsultId);

        // [1] isPerforming（検査実施する）：true & 中止理由：null
        var performingList = request.Executions.Where(x => x.IsPerforming && x.CancelReasonId is null).ToArray();

        // [2] isPerforming（検査実施する）：false & 中止理由：あり
        var notPerformingList = request.Executions.Where(x => !x.IsPerforming && x.CancelReasonId is not null).ToArray();

        // [1]-a 削除対象
        // 保存済みの中止レコードに対して検査項目IDで突合して、削除対象の検査項目明細IDを取得する
        var removeTargets = performingList.SelectMany(req => examCancels.ExamItemDetailCancels
                                                                .Where(x => x.ExamItemId == req.ExamItemId)
                                                                .Select(x => x.ExamItemDetailId)
                                                     ).ToArray();

        // [2]-a,b
        // UPSERTはリポジトリに任せる
        var toSave = notPerformingList.Select(x => new Domain.Models.ExamItemCancel()
        {
            ExamItemId = x.ExamItemId,
            CancelReasonId = (int)x.CancelReasonId!,
        }).ToArray();

        await _consultRepository.RemoveExamCancelsAsync(consult.ConsultId, removeTargets);
        await _consultRepository.SaveExamCancelsAsync(consult.ConsultId, toSave);
    }
    /// <summary>
    /// 検査結果入力情報を取得する
    /// </summary>
    public async Task<InputExamItems> GetInputExamItemsExamineeAsync(string consultNumber, int examMenuId)
    {
        var consult = await _consultRepository.GetConsultAsync(consultNumber);
        var examinee = await _examineeRepository.GetExamineeAsync(consult.ExamineeId);
        // 会場日程IDを指定して会場日程を取得する
        var placeSchedule = await _placeScheduleRepository.GetPlaceScheduleAsync(consult.PlaceScheduleId);
        // 健診日
        DateOnly examDate = placeSchedule.ExamDate;
        // 受診日の年齢
        // NOTE: 年齢加算日は暫定で前日年齢加算
        Domain.Models.Age examAge = examinee.Birthdate.GetAge(examDate, Core.Enums.AgeCalcMode.前日年齢加算);
        // 検査メニューに関連した検査項目情報を取得
        var examItemGroups = await _examItemRepository.GetExamItemGroupsAsync(examMenuId);
        // 検査項目明細IDを取得
        var examItemDetailIds = examItemGroups.SelectMany(group => group.ExamItems)
                                              .SelectMany(item => item.ExamItemDetails)
                                              .Select(detail => detail.ExamItemDetailId)
                                              .ToArray();
        // キーボード入力値リスト
        var Keyboards = await _examItemRepository.GetKeyboardOptionssAsync(examItemDetailIds);
        // 検査項目明細選択肢
        var examItemDetailOptions = await _examItemRepository.GetExamItemDetailOptionsAsync(examItemDetailIds);
        // 基準値パターンIDを取得
        var thresholds = await _consultRepository.GetConsultThresholds(consult.ConsultId);
        // 検査正常値範囲を取得
        var examNormalValueRanges = await _examItemRepository.GetExamNormalValueRangesAsync(thresholds.ToArray(), examItemDetailIds);
        // 検査正常値範囲を対象年齢・対象性別で絞り込む
        List<Domain.Models.ExamNormalValueRange> normalValueRanges = new List<Domain.Models.ExamNormalValueRange>();
        foreach (var normatValue in examNormalValueRanges.Where(x => ((int)x.TargetSex & (int)examinee.Sex) == (int)examinee.Sex))
        {
            var targetAge = new Domain.Models.TargetAge(normatValue.MinAge, normatValue.MaxAge);
            if (targetAge.IsTargetAge(examAge))
            {
                // 対象年齢
                normalValueRanges.Add(normatValue);
            }
        }
        // 検査中止を取得
        var examCancels = await _consultRepository.GetExamCancelsAsync(consult.ConsultId);
        // 検査依頼を取得
        var examOrders = await _consultRepository.GetExamOrdersAsync(consult.ConsultId);
        // 検査結果を取得
        var examResults = await _consultRepository.GetExamResultsAsync(consult.ConsultId);

        return new InputExamItems()
        {
            ConsultNumber = consultNumber,
            Examinee = new InputExamExaminee(){
                TicketNumber = consult.TicketNumber,
                KanaName = examinee.KanaName,
                Sex = (int)examinee.Sex,
                ExamDateAge = examAge.Years
            },
            RelatedExamItems = [],                  // TODO: 関連検査項目 後方作業へ
            ExamItemGroups = 
                examItemGroups.Select(eg => new ExamItemGroup
                {
                    Type = (int)eg.Type,
                    ExamItems = eg.ExamItems.Select(ei => new InputExamItem 
                    {
                        PositionNumber = ei.PositionNumber,
                        ExamItemId = ei.ExamItemId,
                        Name = ei.Name,
                        ExamItemDetails = ei.ExamItemDetails.Select(ed => new ExamItemDetail
                        {
                            PositionNumber = ed.PositionNumber,
                            ExamItemDetailId = ed.ExamItemDetailId,
                            EquipmentLabel = ed.EquipmentLabel,
                            Name = ed.Name,
                            HasOrder = examOrders.ExamItemDetailOrders.Where(x => x.ExamItemDetailId == ed.ExamItemDetailId).Any(),
                            CancelReasonId = examCancels.ExamItemDetailCancels.Where(x => x.ExamItemDetailId == ed.ExamItemDetailId).Any()
                                            ? examCancels.ExamItemDetailCancels.Where(x => x.ExamItemDetailId == ed.ExamItemDetailId)
                                                                               .Select(x => x.CancelReasonId).ElementAt(0) 
                                            : null,
                            Value = examResults.ExamItemDetailResults.Where(x => x.ExamItemDetailId == ed.ExamItemDetailId).Any()
                                            ? examResults.ExamItemDetailResults.Where(x => x.ExamItemDetailId == ed.ExamItemDetailId)
                                                                               .Select(x => x.Value).ElementAt(0)
                                            : "",  
                            PrevValue = "",         // TODO: 前回値
                            Unit = ed.Unit,
                            Type = (int)ed.Type,
                            IntegerLength = ed.IntegerLength,
                            DecimalLength = ed.DecimalLength,
                            // キーボード入力
                            Keyboard = new Keyboard{
                                KeyboardType = (int)ed.KeyboardType,
                                Values = Keyboards.Where(kb => kb.ExamItemDetailId == ed.ExamItemDetailId)
                                                  .OrderBy(kb => kb.OptionId)
                                                  .Select(kb => kb.Value)
                                                  .ToArray()
                            },
                            // 選択肢
                            ExamItemDetailOptions = 
                                examItemDetailOptions.Where(op => op.ExamItemDetailId == ed.ExamItemDetailId)
                                                     .OrderBy(op => op.OrderNumber)         
                                                     .Select(op => new ExamItemDetailOption
                                                        {
                                                            OrderNumber = op.OrderNumber,
                                                            Code = op.Code,
                                                            Name = op.Name
                                                        }).ToArray(),
                            // 検査正常値範囲
                            ExamNormalValueRanges = 
                                normalValueRanges.Where(r => r.ExamItemDetailId == ed.ExamItemDetailId)
                                                 .OrderBy(r => r.Priority)
                                                 .Select(r => new ExamNormalValueRange
                                                 {
                                                    ErrorLevel = (int)r.ErrorLevel,
                                                    MaxValue = r.MaxValue,
                                                    MinValue = r.MinValue
                                                 }).ToArray()
                        }).ToArray(),
                        ExamRegistResults = []      // TODO: 検査結果登録エラー 後方作業へ
                    }).ToArray()
                }).ToArray()
        };
    }
}

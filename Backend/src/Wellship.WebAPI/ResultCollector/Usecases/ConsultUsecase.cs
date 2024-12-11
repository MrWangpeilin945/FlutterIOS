using Ryobi.Wellship.APIModels.Requests;
using Ryobi.Wellship.APIModels.Responses;
using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.Core.Exceptions;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models.Triggers;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

/// <summary>
/// 受診ユースケース
/// </summary>
public class ConsultUsecase : IConsultUsecase
{
    private readonly IConsultRepository _consultRepository;
    private readonly IExamineeRepository _examineeRepository;
    private readonly IExamMenuRepository _examMenuRepository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="consultRepository">受診リポジトリ</param>
    /// <param name="examineeRepository">受診者リポジトリ</param>
    /// <param name="examMenuRepository">検査メニューリポジトリ</param>
    public ConsultUsecase(IConsultRepository consultRepository, IExamineeRepository examineeRepository, IExamMenuRepository examMenuRepository)
    {
        _consultRepository = consultRepository;
        _examineeRepository = examineeRepository;
        _examMenuRepository = examMenuRepository;
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
            UnexaminedMenus = unexaminedConsult.UnexaminedExamMenus.Select(x => new APIModels.Responses.ExamMenu()
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
    /// 前提検査メニューを検証する
    /// 前提検査メニューのうち、未受診の検査メニューがあれば返却する
    /// </summary>
    public async Task<IEnumerable<Domain.Models.ExamMenu>> ValidatePriorExamMenus(string consultNumber, int examMenuId)
    {
        // 未受診の検査メニューを取得する
        var unexaminedList = await _consultRepository.GetUnexaminedConsultsAsync([consultNumber]);
        var unexamined = unexaminedList.SingleOrDefault(x => x.ConsultNumber == consultNumber);

        // 指定した受診について、未受診の検査メニューがない場合は、エラーなし
        if (unexamined is null)
        {
            return [];
        }
        var unexaminedMenuIds = unexamined.UnexaminedExamMenus.Select(x => x.ExamMenuId).ToArray();

        // 前提検査メニューの設定を取得する
        var priorExamMenusSetting = await _examMenuRepository.GetPriorExamMenusAsync(examMenuId);

        // 現在の検査メニューについて、前提検査メニューの設定がない場合はエラーなし
        if (priorExamMenusSetting is null)
        {
            return [];
        }

        // 前提検査が必要な検査メニューを返す
        var missingPriorMenus = priorExamMenusSetting.GetMissingPriorMenus(unexaminedMenuIds);
        return unexamined.UnexaminedExamMenus.Where(x => missingPriorMenus.Contains(x.ExamMenuId))
                                             .Select(x => new Domain.Models.ExamMenu() { MenuId = x.ExamMenuId, MenuName = x.ExamMenuName })
                                             .ToArray();
    }

    /// <summary>
    /// 検査結果相関ルールで検証します。
    /// </summary>
    public async Task<IEnumerable<Domain.Models.RuleError>> ValidateCorrelationRuleAsync(string consultNumber, ResultsRequest result)
    {
        // var consult = await _consultRepository.GetConsultAsync(consultNumber);

        var examMenuId = result.ExamMenuId;

        // Repo1. 検査結果相関ルールマスタを取得する（examMenuId）
        var ruleList = new List<Domain.Models.CorrelationRule>()
        {
            new(){
                CorrelationRuleId = 1,
                Name = "腹囲_前回差20cm以上",
                ExamMenuId = 5,
                Priority = 1,
                TriggerType = RuleTriggerType.ThresholdExceeded,
                ErrorLevel = InputErrorLevel.警告,
                ExamItemId = 5,
                Message = "腹囲が前回より20cm以上です。",
                Evaluations = [
                    new CorrelationRuleEvaluation(){VariableNumber = 1, EvaluationValue = "20"}
                ],
                ExamItemDetails = [
                    new CorrelationRuleExamItemDetail(){VariableNumber = 1, ExamItemDetailId = 5, SourceType = SourceType.今回値},
                    new CorrelationRuleExamItemDetail(){VariableNumber = 2, ExamItemDetailId = 5, SourceType = SourceType.前回値}
                ]
            }
        };

        // Repo2. DBから前回値を取得する（consultId）
        var 前回値として取得が必要な検査項目明細ID = ruleList.SelectMany(x => x.ExamItemDetails)
                                                          .Where(x => x.SourceType == SourceType.前回値)
                                                          .Select(x => x.ExamItemDetailId).Distinct().ToArray();
        var 前回値リスト = new Dictionary<int, string>() {
            { 1, "168" },
            { 5, "60" }
        };

        // Repo3. DBから今回値を取得する
        var 今回値として取得が必要な検査項目明細ID = ruleList.SelectMany(x => x.ExamItemDetails)
                                                          .Where(x => x.SourceType == SourceType.今回値)
                                                          .Select(x => x.ExamItemDetailId).Distinct().ToArray();
        var 今回値リスト = new Dictionary<int, string>() {
            { 1, "169" },
            { 5, "71" }
        };

        // リクエスト値を取得する
        var リクエスト値リスト = result.ExamResults.SelectMany(x => x.ExamItemDetails)
                                                 .ToDictionary(x => x.ExamItemDetailId, x => x.Value);

        // 保存済み今回値とリクエスト値と合成する
        // リクエスト値を優先する
        var 合成済みの今回値リスト = 今回値リスト.Where(x => !リクエスト値リスト.ContainsKey(x.Key))
                                              .Concat(リクエスト値リスト)
                                              .ToDictionary(x => x.Key, x => x.Value);

        // トリガーを作る
        var errors = new List<Domain.Models.RuleError>();
        foreach (var rule in ruleList)
        {
            var triggerType = rule.TriggerType;
            var errorLevel = rule.ErrorLevel;
            var conditionValues = rule.Evaluations.OrderBy(x => x.VariableNumber)
                                                  .Select(x => x.EvaluationValue)
                                                  .ToList();
            var inputValues = rule.ExamItemDetails.OrderBy(x => x.VariableNumber)
                                                  .Select(x => x.SourceType switch
                                                  {
                                                      SourceType.今回値 => 合成済みの今回値リスト.TryGetValue(x.ExamItemDetailId, out var 今回値) ? 今回値 : "",
                                                      SourceType.前回値 => 前回値リスト.TryGetValue(x.ExamItemDetailId, out var 前回値) ? 前回値 : "",
                                                      _ => throw new NotSupportedException(nameof(x.SourceType))
                                                  }).ToList();

            var trigger = TriggerFactory.CreateTrigger(triggerType, inputValues, conditionValues, errorLevel);

            if (trigger.IsMatch())
            {
                errors.Add(new RuleError()
                {
                    ErrorLevel = trigger.GetErrorLevel(),
                    Message = rule.Message,
                    Priority = rule.Priority,
                    ExamItemId = rule.ExamItemId
                });
            }
        }

        // 返却するエラーレベル
        var errorLevels = new List<InputErrorLevel>() { InputErrorLevel.警告, InputErrorLevel.異常 };

        return errors.Where(x => errorLevels.Contains(x.ErrorLevel))
                     .OrderBy(x => x.Priority);
    }
}

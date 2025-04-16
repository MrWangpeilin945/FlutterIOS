using Ryobi.Wellship.APIModels.Requests;
using Ryobi.Wellship.APIModels.Responses;
using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.Core.Exceptions;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models.Triggers;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Auth;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

/// <summary>
/// 受診ユースケース
/// </summary>
public class ConsultUsecase : IConsultUsecase
{
    private readonly IConsultRepository _consultRepository;
    private readonly IExamineeRepository _examineeRepository;
    private readonly IExamMenuRepository _examMenuRepository;
    private readonly IExamItemRepository _examItemRepository;
    private readonly IPlaceScheduleRepository _placeScheduleRepository;
    private readonly IResultRepository _resultRepository;
    private readonly IStaffIdentityProvider _staffIdentityProvider;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="consultRepository">受診リポジトリ</param>
    /// <param name="examineeRepository">受診者リポジトリ</param>
    /// <param name="examMenuRepository">検査メニューリポジトリ</param>
    /// <param name="examItemRepository">検査項目リポジトリ</param>
    /// <param name="placeScheduleRepository">会場日程リポジトリ</param>
    /// <param name="resultRepository">検査結果リポジトリ</param>
    /// <param name="staffIdentityProvider">職員情報プロバイダ</param>
    public ConsultUsecase(IConsultRepository consultRepository, IExamineeRepository examineeRepository, IExamMenuRepository examMenuRepository,
                          IExamItemRepository examItemRepository, IPlaceScheduleRepository placeScheduleRepository, IResultRepository resultRepository,
                          IStaffIdentityProvider staffIdentityProvider)
    {
        _consultRepository = consultRepository;
        _examineeRepository = examineeRepository;
        _examMenuRepository = examMenuRepository;
        _examItemRepository = examItemRepository;
        _placeScheduleRepository = placeScheduleRepository;
        _resultRepository = resultRepository;
        _staffIdentityProvider = staffIdentityProvider;
    }

    /// <summary>
    /// AP1007_受診番号の受診が存在するか確認する
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
    /// AP1008_未受診の検査メニューを取得する
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
    /// AP1010_検査内容を取得する
    /// </summary>
    public async Task<ExamContent> GetExamItemsExamineeAsync(string consultNumber, int examMenuId)
    {
        var consult = await _consultRepository.GetConsultAsync(consultNumber);
        var examinee = await _examineeRepository.GetExamineeAsync(consult.ExamineeId);
        // 会場日程IDを指定して会場日程を取得する
        var placeSchedule = await _placeScheduleRepository.GetPlaceScheduleAsync(consult.PlaceScheduleId);
        // 健診日
        DateOnly examDate = placeSchedule.ExamDate;
        // 受診の年齢
        var examAge = consult.Age;
        // 検査メニューに関連した検査項目情報を取得
        var examItemGroups = await _examItemRepository.GetExamItemGroupsAsync(examMenuId);
        // 検査中止を取得
        var examCancels = await _consultRepository.GetExamCancelsAsync(consult.ConsultId);
        // 検査依頼を取得
        var examOrders = await _consultRepository.GetExamOrdersAsync(consult.ConsultId);
        // 未受診の検査メニューを取得
        var unexaminedItems = await GetUnexaminedMenusAsync(consultNumber);
        // 同姓同名アラート
        var sameNameAlert = await _placeScheduleRepository.IsSamenameAsync(consultNumber);

        // 検査項目明細IDを取得
        var examItemDetailIds = examItemGroups.SelectMany(group => group.ExamItems)
                                              .SelectMany(item => item.ExamItemDetails)
                                              .Select(detail => detail.ExamItemDetailId)
                                              .ToArray();
        // 受診特記一覧を取得
        var consultNotes = await _consultRepository.GetConsultNotesAsync(consult.ConsultId);

        // 検査結果を取得
        var examResults = await _consultRepository.GetExamResultsAsync(consult.ConsultId);

        // 過去検査結果を取得
        var previousResults = await _consultRepository.GetPreviousResultsAsync(consult.ConsultId, examDate);

        // 検査メニュー特記一覧を取得
        var menuNotes = await _examMenuRepository.GetMenuNotesAsync(examMenuId);
        var menuNoteDetailIds = menuNotes.SelectMany(note => note.ExamResults).Select(r => r.ExamItemDetailId).Distinct().ToArray();
        var menuNoteDetailChildren = await _examItemRepository.GetExamItemDetailChildrenAsync(menuNoteDetailIds);
        var examNoteResults = menuNotes.Select(x => new Domain.Models.MenuNoteResult(x, menuNoteDetailChildren, examResults, previousResults, consultNotes)).ToArray();

        // 関連検査項目を取得
        var relatedExamItems = examNoteResults.Select(x => new RelatedExamItem()
        {
            ExamItemName = x.MenuNoteName,
            ExamResult = x.GetDisplayText()
        }).ToArray();

        // 検査実施判断ルールで検証する
        var decisionRules = await ValidateDecisionRuleAsync(examMenuId, examResults, previousResults);
        var examDecisionResults = decisionRules.OrderByDescending(x => x.ErrorLevel)
                                               .Select(x => new ExamDecisionResult()
                                               {
                                                   ErrorLevel = (int)x.ErrorLevel,
                                                   Description = x.Message
                                               });

        // 前提検査メニューで検証する
        var priorExamMenus = await ValidatePriorExamMenusAsync(consultNumber, examMenuId);
        var priorExamMenusResults = priorExamMenus.OrderBy(x => x.MenuId)
                                                  .Select(x => new ExamDecisionResult()
                                                  {
                                                      ErrorLevel = (int)InputErrorLevel.異常,
                                                      Description = $"{x.MenuName}が終わっていないため、開始できません。"
                                                  });

        return new ExamContent()
        {
            ConsultNumber = consultNumber,
            ConsultName = consult.Note,
            Examinee = new Examinee()
            {
                TicketNumber = consult.TicketNumber,
                Name = examinee.Name,
                KanaName = examinee.KanaName,
                Birthdate = examinee.Birthdate.Value,
                Sex = (int)examinee.Sex,
                Organizations = examinee.Affiliations.OrderBy(x => x.OrderNumber)
                                                     .Select(x => x.OrganizationName).ToArray(),
                SameNameAlert = sameNameAlert,
                ExamDateAge = examAge.Years
            },
            // 選択した検査メニューが未受診ならばfalseとする
            IsComplete = !unexaminedItems.UnexaminedMenus.Select(x => x.ExamMenuId).Contains(examMenuId),
            RelatedExamItems = relatedExamItems,
            ExamItems = examItemGroups.OrderBy(group => group.ExamItemGroupId)
                                      .SelectMany(group => group.ExamItems)
                                      .OrderBy(item => item.PositionNumber)
                                      .Select(item => new ExamDetail
                                      {
                                          ExamItemId = item.ExamItemId,
                                          ExamItemName = item.Name,
                                          HasOrder = examOrders.ExamItemDetailOrders
                                                             .Any(x => item.ExamItemDetails.Select(d => d.ExamItemDetailId).Contains(x.ExamItemDetailId)),
                                          // 明細単位で記録された中止を検査項目単位に丸める
                                          // 前提：検査項目単位の中止理由が同じである
                                          CancelReasonId = examCancels.ExamItemDetailCancels
                                                                      .Where(x => item.ExamItemDetails.Select(d => d.ExamItemDetailId).Contains(x.ExamItemDetailId))
                                                                      .OrderBy(x => x.ExamItemDetailId)
                                                                      .Select(x => (int?)x.CancelReasonId)
                                                                      .FirstOrDefault()
                                      }).ToArray(),
            ExamDecisionResults = priorExamMenusResults.Union(examDecisionResults).ToArray(),
            UnexaminedItems = unexaminedItems.UnexaminedMenus.Select(x => new ExamMenu
            {
                ExamMenuId = x.ExamMenuId,
                ExamMenuName = x.ExamMenuName
            }).ToArray(),
        };
    }

    /// <summary>
    /// AP1022_検査の実施有無と中止理由を登録する
    /// </summary>
    public async Task RegisterExecutionsAsync(string consultNumber, ExecutionsRequest request)
    {
        // NOTE: 中止理由の登録ルール
        // 前提：リクエストは検査項目単位、DBは検査項目明細単位
        //       会場がロック中の時は管理者のみ操作可能
        // 
        // [1] isPerforming（検査実施する）：true & 中止理由：null
        //   - a. 中止レコードがある => 中止レコードを削除する
        //   - b. 中止レコードがない => 処理しない
        //
        // [2] isPerforming（検査実施する）：false & 中止理由：あり
        //   - a. 中止レコードがある => 中止レコードを更新する
        //   - b. 中止レコードがない => 中止レコードを挿入する

        var consult = await _consultRepository.GetConsultAsync(consultNumber);
        // 会場日程がロック中かを確認
        // ロール：管理者は操作可能
        var placeSchedule = await _placeScheduleRepository.GetPlaceScheduleLockingStatusAsync(consult.PlaceScheduleId);
        if (placeSchedule?.Status == PlaceScheduleLockingStatus.検査完了 && _staffIdentityProvider.Role != Role.Admin)
        {
            // 会場ロック中
            throw new PlaceScheduleLockedException();
        }

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

        await _consultRepository.SaveExamCancelsAsync(consult.ConsultId, removeTargets, toSave);
    }

    /// <summary>
    /// 前提検査メニューを検証する
    /// 前提検査メニューのうち、未受診の検査メニューがあれば返却する
    /// </summary>
    public async Task<IEnumerable<Domain.Models.ExamMenu>> ValidatePriorExamMenusAsync(string consultNumber, int examMenuId)
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
    /// AP1009_検査結果入力情報を取得する
    /// </summary>
    public async Task<InputExamItems> GetInputExamItemsExamineeAsync(string consultNumber, int examMenuId)
    {
        var consult = await _consultRepository.GetConsultAsync(consultNumber);
        var examinee = await _examineeRepository.GetExamineeAsync(consult.ExamineeId);
        // 会場日程IDを指定して会場日程を取得する
        var placeSchedule = await _placeScheduleRepository.GetPlaceScheduleAsync(consult.PlaceScheduleId);
        // 健診日
        DateOnly examDate = placeSchedule.ExamDate;
        // 受診の年齢
        var examAge = consult.Age;
        // 未受診の検査メニューを取得
        var unexaminedItems = await GetUnexaminedMenusAsync(consultNumber);
        // 検査メニューに関連した検査項目情報を取得
        var examItemGroup = await _examItemRepository.GetExamItemGroupsAsync(examMenuId);
        // 検査項目明細IDを取得
        var examItemDetailIds = examItemGroup.SelectMany(group => group.ExamItems)
                                              .SelectMany(item => item.ExamItemDetails)
                                              .Select(detail => detail.ExamItemDetailId)
                                              .ToArray();
        // 検査結果を取得
        var examResults = await _consultRepository.GetExamResultsAsync(consult.ConsultId);
        // 過去検査結果を取得
        var previousResults = await _consultRepository.GetPreviousResultsAsync(consult.ConsultId, examDate);
        // 受診特記一覧を取得
        var consultNotes = await _consultRepository.GetConsultNotesAsync(consult.ConsultId);

        // 検査メニュー特記一覧を取得
        var menuNotes = await _examMenuRepository.GetMenuNotesAsync(examMenuId);
        var menuNoteDetailIds = menuNotes.SelectMany(note => note.ExamResults).Select(r => r.ExamItemDetailId).Distinct().ToArray();
        var menuNoteDetailChildren = await _examItemRepository.GetExamItemDetailChildrenAsync(menuNoteDetailIds);
        var examNoteResults = menuNotes.Select(x => new Domain.Models.MenuNoteResult(x, menuNoteDetailChildren, examResults, previousResults, consultNotes)).ToArray();

        // 関連検査項目を取得
        var relatedExamItems = examNoteResults.Select(x => new RelatedExamItem()
        {
            ExamItemName = x.MenuNoteName,
            ExamResult = x.GetDisplayText()
        }).ToArray();
        // リクエストの検査結果を組み立てる
        var results = new ResultsRequest
        {
            ExamMenuId = examMenuId,
            ExamResults =
                examItemGroup.SelectMany(group => group.ExamItems)
                                .Select(item => new ResultRequest
                                {
                                    ExamItemId = item.ExamItemId,
                                    ExamItemDetails = item.ExamItemDetails.Select(detail => new ExamItemDetailRequest
                                    {
                                        ExamItemDetailId = detail.ExamItemDetailId,
                                        Value = examResults.ExamItemDetailResults.Where(result => result.ExamItemDetailId == detail.ExamItemDetailId)
                                                                                    .SingleOrDefault()?.Value ?? ""
                                    }).ToArray()
                                }).ToArray()
        };
        // 検査結果相関ルールを検証する
        var ruleErrors = await ValidateCorrelationRuleAsync(consultNumber, results);

        // 検査項目グループ情報を取得する
        var examItemGroups = await GetExamItemGroups(consult.ConsultId, examAge, examinee.Sex, examItemGroup, examResults, previousResults, ruleErrors);

        return new InputExamItems()
        {
            ConsultNumber = consultNumber,
            Examinee = new InputExamExaminee()
            {
                TicketNumber = consult.TicketNumber,
                KanaName = examinee.KanaName,
                Sex = (int)examinee.Sex,
                ExamDateAge = examAge.Years
            },
            IsComplete = !unexaminedItems.UnexaminedMenus.Select(x => x.ExamMenuId).Contains(examMenuId),
            RelatedExamItems = relatedExamItems,
            ExamItemGroups = examItemGroups.ToArray()
        };
    }

    /// <summary>
    /// 検査結果相関ルールで検証します。
    /// </summary>
    public async Task<IEnumerable<Domain.Models.RuleError>> ValidateCorrelationRuleAsync(string consultNumber, ResultsRequest result)
    {
        var consult = await _consultRepository.GetConsultAsync(consultNumber);
        var placeSchedule = await _placeScheduleRepository.GetPlaceScheduleAsync(consult.PlaceScheduleId);

        var examMenuId = result.ExamMenuId;

        // 検査結果相関ルールマスタを取得する
        var ruleList = await _examItemRepository.GetCorrelationRulesAsync(examMenuId);

        // DBから前回値と今回値を取得する
        var dbCurrentResults = await _consultRepository.GetExamResultsAsync(consult.ConsultId);
        var dbPreResults = await _consultRepository.GetPreviousResultsAsync(consult.ConsultId, placeSchedule.ExamDate);
        var currentResults = dbCurrentResults.ExamItemDetailResults.ToDictionary(x => x.ExamItemDetailId, x => x.Value);
        var preResults = dbPreResults.ExamItemDetailResults.ToDictionary(x => x.ExamItemDetailId, x => x.Value);

        // リクエスト値を取得する
        var currentRequestResults = result.ExamResults.SelectMany(x => x.ExamItemDetails)
                                                      .ToDictionary(x => x.ExamItemDetailId, x => x.Value);

        // DBの今回値とリクエスト値と合成する（リクエスト値を優先する）
        var concatenatedCurrentResults = currentResults.Where(x => !currentRequestResults.ContainsKey(x.Key))
                                                       .Concat(currentRequestResults)
                                                       .ToDictionary(x => x.Key, x => x.Value);

        // トリガーをセットアップして検証する
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
                                                      SourceType.今回値 => concatenatedCurrentResults.TryGetValue(x.ExamItemDetailId, out var currVal) ? currVal : null,
                                                      SourceType.前回値 => preResults.TryGetValue(x.ExamItemDetailId, out var prevVal) ? prevVal : null,
                                                      _ => throw new NotSupportedException(nameof(x.SourceType))
                                                  }).ToList();

            // 結果レコードがない場合はトリガーを無効にする
            if (inputValues.Any(x => x is null))
            {
                continue;
            }

            var trigger = TriggerFactory.CreateTrigger(triggerType, inputValues!, conditionValues, errorLevel);

            // トリガーの条件に一致すればエラーに追加する
            if (trigger.IsMatch())
            {
                errors.Add(new Domain.Models.RuleError()
                {
                    ErrorLevel = trigger.GetErrorLevel(),
                    Message = rule.Message,
                    Priority = rule.Priority,
                    ExamItemId = rule.ExamItemId
                });
            }
        }

        // 条件に一致したトリガーのうち、エラーレベルが警告と異常の結果のみ返す
        var errorLevels = new List<InputErrorLevel>() { InputErrorLevel.警告, InputErrorLevel.異常 };
        return errors.Where(x => errorLevels.Contains(x.ErrorLevel))
                     .OrderBy(x => x.Priority);
    }

    /// <summary>
    /// 検査実施判断ルールで検証します。
    /// </summary>
    public async Task<IEnumerable<Domain.Models.RuleError>> ValidateDecisionRuleAsync(int examMenuId,
                                                                                      Domain.Models.ExamResult currentResult,
                                                                                      Domain.Models.PreviousResult previousResult)
    {
        // 検査実施判断ルールマスタを取得する
        var ruleList = await _examItemRepository.GetDecisionRulesAsync(examMenuId);

        // DBから前回値と今回値を取得する
        var currentResults = currentResult.ExamItemDetailResults.ToDictionary(x => x.ExamItemDetailId, x => x.Value);
        var preResults = previousResult.ExamItemDetailResults.ToDictionary(x => x.ExamItemDetailId, x => x.Value);

        // トリガーをセットアップして検証する
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
                                                      SourceType.今回値 => currentResults.TryGetValue(x.ExamItemDetailId, out var currVal) ? currVal : null,
                                                      SourceType.前回値 => preResults.TryGetValue(x.ExamItemDetailId, out var prevVal) ? prevVal : null,
                                                      _ => throw new NotSupportedException(nameof(x.SourceType))
                                                  }).ToList();

            // 結果レコードがない場合はトリガーを無効にする
            if (inputValues.Any(x => x is null))
            {
                continue;
            }

            var trigger = TriggerFactory.CreateTrigger(triggerType, inputValues!, conditionValues, errorLevel);

            // トリガーの条件に一致すればエラーに追加する
            if (trigger.IsMatch())
            {
                errors.Add(new Domain.Models.RuleError()
                {
                    ErrorLevel = trigger.GetErrorLevel(),
                    Message = rule.Message,
                    Priority = rule.Priority
                });
            }
        }

        // 条件に一致したトリガーのうち、エラーレベルが警告と異常の結果のみ返す
        var errorLevels = new List<InputErrorLevel>() { InputErrorLevel.警告, InputErrorLevel.異常 };
        return errors.Where(x => errorLevels.Contains(x.ErrorLevel))
                     .OrderBy(x => x.Priority);
    }

    /// <summary>
    /// 検査基準値を検証する
    /// </summary>
    public async Task<IEnumerable<Domain.Models.RangeError>> ValidateNormalValueRangeAsync(string consultNumber, ResultsRequest result)
    {
        var consult = await _consultRepository.GetConsultAsync(consultNumber);
        var examinee = await _examineeRepository.GetExamineeAsync(consult.ExamineeId);
        var placeSchedule = await _placeScheduleRepository.GetPlaceScheduleAsync(consult.PlaceScheduleId);

        // DBとリクエスト値から今回値を取得して合成する（リクエスト値を優先する）
        var dbCurrentResults = await _consultRepository.GetExamResultsAsync(consult.ConsultId);
        var dbCurrentMap = dbCurrentResults.ExamItemDetailResults.ToDictionary(x => x.ExamItemDetailId, x => x.Value);
        var requestMap = result.ExamResults.SelectMany(x => x.ExamItemDetails).ToDictionary(x => x.ExamItemDetailId, x => x.Value);
        var currentMap = requestMap.Concat(dbCurrentMap.Where(x => !requestMap.ContainsKey(x.Key)))
                                   .ToDictionary(x => x.Key, x => x.Value);

        var examItemDetailIds = currentMap.Select(x => x.Key).ToArray();

        // 検査基準値範囲を取得
        var ranges = await _consultRepository.GetExamNormalValueRangesAsync(consult.ConsultId, examItemDetailIds);
        var filteredRanges = ranges.Where(x => x.TargetAge.IsMatch(consult.Age))
                                   .Where(x => x.TargetSex.IsMatch(examinee.Sex))
                                   .ToArray();

        // 今回値に対して範囲チェック
        var errors = new List<Domain.Models.RangeError>();
        foreach (var current in currentMap)
        {
            // 検査結果がMin以上Max未満に当てはまる範囲の設定を取得する
            // 優先度の昇順でソートして先頭の基準値範囲を採用する
            var range = filteredRanges.Where(x => x.ExamItemDetailId == current.Key && x.ValueRange.InRange(current.Value))
                              .OrderBy(x => x.Priority)
                              .FirstOrDefault();

            if (range is not null)
            {
                errors.Add(new Domain.Models.RangeError()
                {
                    Name = range.Name,
                    ExamItemDetailId = range.ExamItemDetailId,
                    MinValue = range.ValueRange.MinValue,
                    MaxValue = range.ValueRange.MaxValue,
                    ErrorLevel = range.ErrorLevel
                });
            }
        }

        // 条件に一致した範囲のうち、エラーレベルが警告と異常の結果のみ返す
        var errorLevels = new List<InputErrorLevel>() { InputErrorLevel.警告, InputErrorLevel.異常 };
        return errors.Where(x => errorLevels.Contains(x.ErrorLevel))
                     .OrderByDescending(x => x.ErrorLevel);
    }
    /// <summary>
    /// AP1014_検査結果を登録する
    /// </summary>
    public async Task RegisterResultsAsync(string consultNumber, ResultsRequest results)
    {
        var consult = await _consultRepository.GetConsultAsync(consultNumber);
        // 会場日程がロック中かを確認
        // ロール：管理者は操作可能
        var placeSchedule = await _placeScheduleRepository.GetPlaceScheduleLockingStatusAsync(consult.PlaceScheduleId);
        if (placeSchedule?.Status == PlaceScheduleLockingStatus.検査完了 && _staffIdentityProvider.Role != Role.Admin)
        {
            // 会場ロック中
            throw new PlaceScheduleLockedException();
        }
        var examItemIds = results.ExamResults.Select(x => x.ExamItemId);
        var examItemDetailIds = results.ExamResults.SelectMany(x => x.ExamItemDetails).Select(x => x.ExamItemDetailId);

        // 検査結果相関ルールを検証する
        var ruleErrors = await ValidateCorrelationRuleAsync(consultNumber, results);
        // NOTE: リクエストに含まれる検査項目IDで相関ルールエラーをチェックする
        var registrationDeniedRuleError = ruleErrors.Where(x => examItemIds.Contains(x.ExamItemId ?? -1))
                                                    .Where(x => x.ErrorLevel == InputErrorLevel.異常);
        if (registrationDeniedRuleError.Any())
        {
            throw new ExamResultRegistrationErrorException();
        }

        // 検査基準値を検証する
        var rangeErrors = await ValidateNormalValueRangeAsync(consultNumber, results);
        // NOTE: リクエストに含まれる検査項目明細IDで検査基準値エラーをチェックする
        var registrationDeniedRangeError = rangeErrors.Where(x => examItemDetailIds.Contains(x.ExamItemDetailId))
                                                      .Where(x => x.ErrorLevel == InputErrorLevel.異常);
        if (registrationDeniedRangeError.Any())
        {
            throw new ExamResultRegistrationErrorException();
        }
        var resultList = results.ExamResults.SelectMany(exam => exam.ExamItemDetails)
                                            .Select(detail => new ExamResultRegisteEntity
                                            {
                                                ExamItemDetailId = detail.ExamItemDetailId,
                                                Value = detail.Value
                                            })
                                            .ToArray();

        // 登録と履歴を書き込む
        await _resultRepository.RegisterResultsAsync(consult.ConsultId, resultList);
        await _resultRepository.WriteResultsLogAsync(consult.ConsultId, resultList);
    }

    /// <summary>
    /// AP1013_検査結果を検証する
    /// </summary>
    public async Task<VerifyExamItems> VerifyResults(string consultNumber, ResultsRequest results)
    {
        // 受診を取得
        var consult = await _consultRepository.GetConsultAsync(consultNumber);
        var examinee = await _examineeRepository.GetExamineeAsync(consult.ExamineeId);
        // 会場日程IDを指定して会場日程を取得する
        var placeSchedule = await _placeScheduleRepository.GetPlaceScheduleAsync(consult.PlaceScheduleId);
        // 健診日
        DateOnly examDate = placeSchedule.ExamDate;
        // 受診の年齢
        var examAge = consult.Age;
        // 検査結果を取得
        var examResults = await _consultRepository.GetExamResultsAsync(consult.ConsultId);
        // 過去検査結果を取得
        var previousResults = await _consultRepository.GetPreviousResultsAsync(consult.ConsultId, examDate);
        // 検査メニューに関連した検査項目情報を取得
        var examItemGroup = await _examItemRepository.GetExamItemGroupsAsync(results.ExamMenuId);
        // 検査結果相関ルールを検証する
        var ruleErrors = await ValidateCorrelationRuleAsync(consultNumber, results);
        // 検査項目グループ情報を取得する
        var examItemGroups = await GetExamItemGroups(consult.ConsultId, examAge, examinee.Sex, examItemGroup, examResults, previousResults, ruleErrors);

        return new VerifyExamItems
        {
            // 検査項目グループ情報を取得する
            ExamItemGroups = examItemGroups.ToArray()
        };
    }

    /// <summary>
    /// AP1025_検査結果を取り消す
    /// </summary>
    public async Task BatchDeleteResultsAsync(string consultNumber, ResultDeleteRequest resultDeleteRequest)
    {
        var consult = await _consultRepository.GetConsultAsync(consultNumber);
        await _resultRepository.BatchDeleteResultsAsync(consult.ConsultId, resultDeleteRequest.ExamItemDetailIds);
    }

    /// <summary>
    /// 検査項目グループ情報を取得する
    /// </summary>
    private async Task<IEnumerable<APIModels.Responses.ExamItemGroup>> GetExamItemGroups(Guid consultId, Domain.Models.Age examAge, Sex sex, IEnumerable<Domain.Models.ExamItemGroup> examItemGroup,
                                                                                         Domain.Models.ExamResult examResults, Domain.Models.PreviousResult previousResults,
                                                                                         IEnumerable<Domain.Models.RuleError> ruleErrors)
    {
        // 検査項目明細IDを取得
        var examItemDetailIds = examItemGroup.SelectMany(group => group.ExamItems)
                                              .SelectMany(item => item.ExamItemDetails)
                                              .Select(detail => detail.ExamItemDetailId)
                                              .ToArray();
        // キーボード入力値リスト
        var Keyboards = await _examItemRepository.GetKeyboardOptionssAsync(examItemDetailIds);
        // 検査項目明細選択肢
        var examItemDetailOptions = await _examItemRepository.GetExamItemDetailOptionsAsync(examItemDetailIds);

        // 検査基準値範囲を取得
        var examNormalValueRanges = await _consultRepository.GetExamNormalValueRangesAsync(consultId, examItemDetailIds);
        var filteredRanges = examNormalValueRanges.Where(x => x.TargetAge.IsMatch(examAge))
                                                  .Where(x => x.TargetSex.IsMatch(sex));


        // 検査中止を取得
        var examCancels = await _consultRepository.GetExamCancelsAsync(consultId);
        // 検査依頼を取得
        var examOrders = await _consultRepository.GetExamOrdersAsync(consultId);

        return examItemGroup.Select(eg => new APIModels.Responses.ExamItemGroup
        {
            Type = (int)eg.Type,
            ExamItems = eg.ExamItems.Select(ei => new InputExamItem
            {
                PositionNumber = ei.PositionNumber,
                ExamItemId = ei.ExamItemId,
                Name = ei.Name,
                ExamItemDetails = ei.ExamItemDetails.Select(ed => new APIModels.Responses.ExamItemDetail
                {
                    PositionNumber = ed.PositionNumber,
                    ExamItemDetailId = ed.ExamItemDetailId,
                    EquipmentLabel = ed.EquipmentLabel,
                    Name = ed.Name,
                    HasOrder = examOrders.ExamItemDetailOrders.Any(x => x.ExamItemDetailId == ed.ExamItemDetailId),
                    CancelReasonId = examCancels.ExamItemDetailCancels.SingleOrDefault(x => x.ExamItemDetailId == ed.ExamItemDetailId)?.CancelReasonId,
                    Value = examResults.ExamItemDetailResults.SingleOrDefault(x => x.ExamItemDetailId == ed.ExamItemDetailId)?.Value ?? "",
                    PrevValue = previousResults.ExamItemDetailResults.SingleOrDefault(x => x.ExamItemDetailId == ed.ExamItemDetailId)?.Value ?? "",
                    Unit = ed.Unit,
                    Type = (int)ed.Type,
                    IntegerLength = ed.IntegerLength,
                    DecimalLength = ed.DecimalLength,
                    // キーボード入力
                    Keyboard = new APIModels.Responses.Keyboard
                    {
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
                                                .Select(op => new APIModels.Responses.ExamItemDetailOption
                                                {
                                                    OrderNumber = op.OrderNumber,
                                                    Code = op.Code,
                                                    Name = op.Name
                                                }).ToArray(),
                    // 検査基準値範囲
                    // 優先順位が若い方が優先、次にエラーレベルが高い方が上に
                    ExamNormalValueRanges =
                        filteredRanges.Where(r => r.ExamItemDetailId == ed.ExamItemDetailId)
                                      .OrderBy(r => r.Priority)
                                      .ThenByDescending(r => r.ErrorLevel)
                                      .Select(r => new APIModels.Responses.ExamNormalValueRange
                                      {
                                          ErrorLevel = (int)r.ErrorLevel,
                                          MaxValue = r.ValueRange.MaxValue,
                                          MinValue = r.ValueRange.MinValue
                                      }).ToArray()
                }).ToArray(),
                // 検査結果相関ルール
                ExamRegistResults = ruleErrors.Where(rule => ei.ExamItemId == rule.ExamItemId)
                                              .Select(rule => new ExamRegistResult
                                              {
                                                  ErrorLevel = (int)rule.ErrorLevel,
                                                  Description = rule.Message
                                              })
                                              .OrderByDescending(x => x.ErrorLevel)
                                              .ToArray()
            }).ToArray()
        }).ToArray();
    }

    /// <summary>
    /// AP1026_検査結果を取得する
    /// </summary>
    public async Task<ConsultAllExamResult> GetConsultAllResult(string consultNumber)
    {
        // 受診情報を取得
        var consult = await _consultRepository.GetConsultAsync(consultNumber);

        // 受診者情報を取得
        var examinee = await _examineeRepository.GetExamineeAsync(consult.ExamineeId);
        var sameNameAlert = await _placeScheduleRepository.IsSamenameAsync(consultNumber);
        var examAge = consult.Age;

        // 全ての検査結果を取得
        var consultAllResult = await _resultRepository.GetConsultAllResults(consultNumber);

        return new ConsultAllExamResult
        {
            ConsultNumber = consult.ConsultNumber,
            ConsultName = consult.Note,
            Examinee = new Examinee()
            {
                TicketNumber = consult.TicketNumber,
                Name = examinee.Name,
                KanaName = examinee.KanaName,
                Birthdate = examinee.Birthdate.Value,
                Sex = (int)examinee.Sex,
                Organizations = examinee.Affiliations.OrderBy(x => x.OrderNumber)
                                                     .Select(x => x.OrganizationName).ToArray(),
                SameNameAlert = sameNameAlert,
                ExamDateAge = examAge.Years
            },
            ExamResults = consultAllResult.Select(menu => new DisplayExamResultMenu
            {
                ExamMenuId = menu.ExamMenuId,
                ExamMenuName = menu.ExamMenuName,
                ExamItems = menu.ExamItems.Select(item => new DisplayExamResultItem
                {
                    ExamItemId = item.ExamItemId,
                    ExamItemName = item.ExamItemName,
                    ExamItemDetails = item.ExamItemDetails.Select(itemDetail => new DisplayExamResultItemDetail
                    {
                        ExamItemDetailId = itemDetail.ExamItemDetailId,
                        ExamItemDetailName = itemDetail.ExamItemDetailName,
                        CurrentResult = itemDetail.CurrentResult ?? "",
                        PastResult = itemDetail.PastResult ?? "",
                        PastDate = itemDetail.PastDate ?? null,
                        IsRecent = itemDetail.IsRecent,
                        Status = (int)itemDetail.Status
                    }).ToArray()
                }).ToArray()
            }).ToArray()
        };
    }
}

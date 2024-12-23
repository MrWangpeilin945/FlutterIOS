using Ryobi.Wellship.APIModels.Requests;
using Ryobi.Wellship.APIModels.Responses;
using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.Core.Exceptions;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models.Triggers;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;
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

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="consultRepository">受診リポジトリ</param>
    /// <param name="examineeRepository">受診者リポジトリ</param>
    /// <param name="examMenuRepository">検査メニューリポジトリ</param>
    /// <param name="examItemRepository">検査項目リポジトリ</param>
    /// <param name="placeScheduleRepository">会場日程リポジトリ</param>
    public ConsultUsecase(IConsultRepository consultRepository, IExamineeRepository examineeRepository, IExamMenuRepository examMenuRepository,
                          IExamItemRepository examItemRepository, IPlaceScheduleRepository placeScheduleRepository, IResultRepository resultRepository)
    {
        _consultRepository = consultRepository;
        _examineeRepository = examineeRepository;
        _examMenuRepository = examMenuRepository;
        _examItemRepository = examItemRepository;
        _placeScheduleRepository = placeScheduleRepository;
        _resultRepository = resultRepository;
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
    /// 検査内容を取得する
    /// </summary>
    public async Task<ExamContent> GetExamItemsExamineeAsync(string consultNumber, int examMenuId)
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
        // 検査項目明細マスタ一覧を取得
        var examItemDetailChildren = await _examItemRepository.GetExamItemDetailChildrenAsync(examItemDetailIds);
        // 過去検査結果を取得
        var previousResults = await _consultRepository.GetPreviousResultsAsync(consult.ConsultId, examDate);
        // 検査メニュー特記一覧を取得
        var menuNotes  = await _examMenuRepository.GetMenuNotesAsync(examMenuId);
        var examResults = await _consultRepository.GetExamResultsAsync(consult.ConsultId);
        var examNoteResults = menuNotes .Select(x => new Domain.Models.MenuNoteResult(x, examItemDetailChildren, examResults, previousResults, consultNotes)).ToArray();
        // 関連検査項目を取得
        var relatedExamItems = examNoteResults.Select(x => new RelatedExamItem()
        {
            ExamItemName = x.MenuNoteName,
            ExamResult = x.GetDisplayText()
        }).ToArray();

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
            IsComplete = true,              // TODO: 検査実施判断    後方作業へ
            RelatedExamItems = relatedExamItems,
            ExamItems = examItemGroups.OrderBy(group => group.ExamItemGroupId)
                                      .SelectMany(group => group.ExamItems)
                                      .OrderBy(Item => Item.PositionNumber)
                                      .Select(item => new ExamDetail
                                      {
                                          ExamItemId = item.ExamItemId,
                                          ExamItemName = item.Name,
                                          HasOrder = examOrders.ExamItemDetailOrders
                                                             .Any(x => item.ExamItemDetails.Select(d => d.ExamItemDetailId).Contains(x.ExamItemDetailId)),
                                          CancelReasonId = examCancels.ExamItemDetailCancels
                                                                    .SingleOrDefault(x => item.ExamItemDetails.Select(d => d.ExamItemDetailId).Contains(x.ExamItemDetailId))?.CancelReasonId
                                      }).ToArray(),
            ExamDecisionResult = [],        // TODO: 検査実施判断結果    後方作業へ
            UnexaminedItems = unexaminedItems.UnexaminedMenus.Select(x => new ExamMenu
            {
                ExamMenuId = x.ExamMenuId,
                ExamMenuName = x.ExamMenuName
            }).ToArray(),
        };
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
        IEnumerable<Domain.Models.ExamNormalValueRange> examNormalValueRanges = [];
        if (thresholds.Any())
        {
            examNormalValueRanges = await _examItemRepository.GetExamNormalValueRangesAsync(thresholds.ToArray(), examItemDetailIds, examAge, examinee.Sex);
        }
        // 検査中止を取得
        var examCancels = await _consultRepository.GetExamCancelsAsync(consult.ConsultId);
        // 検査依頼を取得
        var examOrders = await _consultRepository.GetExamOrdersAsync(consult.ConsultId);
        // 検査結果を取得
        var examResults = await _consultRepository.GetExamResultsAsync(consult.ConsultId);
        // 過去検査結果を取得
        var previousResults = await _consultRepository.GetPreviousResultsAsync(consult.ConsultId, examDate);
        // 受診特記一覧を取得
        var consultNotes = await _consultRepository.GetConsultNotesAsync(consult.ConsultId);
        // 検査項目明細マスタ一覧を取得
        var examItemDetailChildren = await _examItemRepository.GetExamItemDetailChildrenAsync(examItemDetailIds);
        // 検査メニュー特記一覧を取得
        var menuNotes = await _examMenuRepository.GetMenuNotesAsync(examMenuId);
        var examNoteResults = menuNotes.Select(x => new Domain.Models.MenuNoteResult(x, examItemDetailChildren, examResults, previousResults, consultNotes)).ToArray();
        // 関連検査項目を取得
        var relatedExamItems = examNoteResults.Select(x => new RelatedExamItem()
        {
            ExamItemName = x.MenuNoteName,
            ExamResult = x.GetDisplayText()
        }).ToArray();

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
            RelatedExamItems = relatedExamItems,
            ExamItemGroups =
                examItemGroups.Select(eg => new APIModels.Responses.ExamItemGroup
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
                            // 検査正常値範囲
                            ExamNormalValueRanges =
                                examNormalValueRanges.Where(r => r.ExamItemDetailId == ed.ExamItemDetailId)
                                                     .OrderBy(r => r.ErrorLevel)
                                                     .Select(r => new APIModels.Responses.ExamNormalValueRange
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
                                                      SourceType.今回値 => concatenatedCurrentResults.TryGetValue(x.ExamItemDetailId, out var currVal) ? currVal : "",
                                                      SourceType.前回値 => preResults.TryGetValue(x.ExamItemDetailId, out var prevVal) ? prevVal : "",
                                                      _ => throw new NotSupportedException(nameof(x.SourceType))
                                                  }).ToList();

            var trigger = TriggerFactory.CreateTrigger(triggerType, inputValues, conditionValues, errorLevel);

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
    /// 検査正常値を検証する
    /// </summary>
    public async Task<IEnumerable<Domain.Models.RangeError>> ValidateNormalValueRangeAsync(string consultNumber, ResultsRequest result)
    {
        var consult = await _consultRepository.GetConsultAsync(consultNumber);
        var examinee = await _examineeRepository.GetExamineeAsync(consult.ExamineeId);
        var placeSchedule = await _placeScheduleRepository.GetPlaceScheduleAsync(consult.PlaceScheduleId);
        var examDate = placeSchedule.ExamDate;
        // 受診日の年齢
        // NOTE: 年齢加算日は暫定で前日年齢加算
        var examAge = examinee.Birthdate.GetAge(examDate, Core.Enums.AgeCalcMode.前日年齢加算);

        // DBとリクエスト値から今回値を取得して合成する（リクエスト値を優先する）
        var dbCurrentResults = await _consultRepository.GetExamResultsAsync(consult.ConsultId);
        var dbCurrentMap = dbCurrentResults.ExamItemDetailResults.ToDictionary(x => x.ExamItemDetailId, x => x.Value);
        var requestMap = result.ExamResults.SelectMany(x => x.ExamItemDetails).ToDictionary(x => x.ExamItemDetailId, x => x.Value);
        var currentMap = requestMap.Concat(dbCurrentMap.Where(x => !requestMap.ContainsKey(x.Key)))
                                   .ToDictionary(x => x.Key, x => x.Value);

        var examItemDetailIds = currentMap.Select(x => x.Key).ToArray();

        // 検査正常値範囲を取得（年齢と性別による絞り込み）
        var thresholds = await _consultRepository.GetConsultThresholds(consult.ConsultId);
        var ranges = await _examItemRepository.GetExamNormalValueRangesAsync(thresholds.ToArray(), examItemDetailIds, examAge, examinee.Sex);

        // 今回値に対して範囲チェック
        var errors = new List<Domain.Models.RangeError>();
        foreach (var current in currentMap)
        {
            // 検査結果がMin以上Max未満に当てはまる範囲の設定を取得する
            var range = ranges.Where(x => x.ExamItemDetailId == current.Key && x.ValueInRange(current.Value))
                              .OrderBy(x => x.Priority)
                              .FirstOrDefault();

            if (range is not null)
            {
                errors.Add(new Domain.Models.RangeError()
                {
                    Name = range.Name,
                    ExamItemDetailId = range.ExamItemDetailId,
                    MinValue = range.MinValue,
                    MaxValue = range.MaxValue,
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
    /// 検査結果を登録する
    /// </summary>
    public async Task RegisterResultsAsync(string consultNumber, ResultsRequest results)
    {
        var consult = await _consultRepository.GetConsultAsync(consultNumber);
        // 会場のロック中かを確認
        // TODO: 管理者のみ更新可能 後方作業へ
        var placeSchedule = await _placeScheduleRepository.GetPlaceScheduleLockingStatusAsync(consult.PlaceScheduleId);
        if (placeSchedule?.Status == PlaceScheduleLockingStatus.検査完了)
        {
            // 会場ロック中
            throw new PlaceScheduleLockedException();
        }
        var resultList = results.ExamResults.SelectMany(exam => exam.ExamItemDetails)
                                            .Select(detail => new ExamResultRegisteEntity{
                                                ExamItemDetailId = detail.ExamItemDetailId,
                                                Value = detail.Value
                                            })
                                            .ToArray();
        await _resultRepository.RegisterResultsAsync(consult.ConsultId, resultList);
    }

    /// <summary>
    /// 検査結果を検証する
    /// </summary>
    public void VerifyResults()
    {
        
    }
}

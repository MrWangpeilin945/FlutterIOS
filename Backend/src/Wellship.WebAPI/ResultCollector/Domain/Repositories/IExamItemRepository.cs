using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

/// <summary>
/// 検査項目リポジトリ
/// </summary>
public interface IExamItemRepository
{
    /// <summary>
    /// 検査メニューに関連した検査項目情報を取得します。
    /// </summary>
    /// <param name="examMenuId">検査メニューID</param>
    Task<IEnumerable<Models.ExamItemGroup>> GetExamItemGroupsAsync(int examMenuId);

    /// <summary>
    /// キーボード入力値リストを取得します。
    /// </summary>
    /// <param name="examItemDetailIds">検査項目明細ID</param>
    Task<IEnumerable<Models.Keyboard>> GetKeyboardOptionssAsync(int[] examItemDetailIds);

    /// <summary>
    /// 検査項目明細選択肢を取得します。
    /// </summary>
    /// <param name="examItemDetailIds">検査項目明細ID</param>
    Task<IEnumerable<Models.ExamItemDetailOption>> GetExamItemDetailOptionsAsync(int[] examItemDetailIds);

    /// <summary>
    /// 検査正常値範囲を取得します。
    /// </summary>
    /// <param name="thresholdIds">基準値パターンID</param>
    /// <param name="examItemDetailIds">検査項目明細ID</param>
    /// <param name="age">受診者の健診時の年齢</param>
    /// <param name="sex">受診者の性別</param>
    Task<IEnumerable<Models.ExamNormalValueRange>> GetExamNormalValueRangesAsync(Guid[] thresholdIds, int[] examItemDetailIds, Age age, Sex sex);

    /// <summary>
    /// 検査結果相関ルールを取得します。
    /// </summary>
    /// <param name="examMenuId">検査メニューID</param>
    Task<IEnumerable<Models.CorrelationRule>> GetCorrelationRulesAsync(int examMenuId);

    /// <summary>
    /// 検査実施判断ルールを取得します。
    /// </summary>
    /// <param name="examMenuId">検査メニューID</param>
    Task<IEnumerable<Models.DecisionRule>> GetDecisionRulesAsync(int examMenuId);

    /// <summary>
    /// 検査項目明細とその子要素を取得します。
    /// </summary>
    /// <param name="examItemDetailIds">検査項目明細ID</param>
    Task<IEnumerable<Models.ExamItemDetailChild>> GetExamItemDetailChildrenAsync(IEnumerable<int> examItemDetailIds);
}

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 前提検査メニューのルール
/// </summary>
public class PriorExamMenu
{
    private readonly IEnumerable<int> _priorExamMenuIdList;
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="currentExamMenuId">現在の検査メニューID</param>
    /// <param name="priorExamMenuIdList">前提検査メニューID一覧。事前に受診済みであることを期待するもの</param>
    public PriorExamMenu(int currentExamMenuId, IEnumerable<int> priorExamMenuIdList)
    {
        CurrentExamMenuId = currentExamMenuId;
        _priorExamMenuIdList = priorExamMenuIdList;
    }
    /// <summary>
    /// 現在の検査メニューID
    /// </summary>
    public int CurrentExamMenuId { get; }
    /// <summary>
    /// 不足している前提検査メニューを取得する
    /// </summary>
    public IEnumerable<int> GetMissingPriorMenus(IEnumerable<int> unexaminedMenuIds)
    {
        return _priorExamMenuIdList.Intersect(unexaminedMenuIds);
    }
}

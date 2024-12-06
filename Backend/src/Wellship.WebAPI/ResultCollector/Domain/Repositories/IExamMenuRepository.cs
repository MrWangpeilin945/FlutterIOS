namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

/// <summary>
/// 検査メニューリポジトリ
/// </summary>
public interface IExamMenuRepository
{
    /// <summary>
    /// 検査メニューを取得します。
    /// テナントに設定されているすべての検査メニューを表示順昇順で取得します。 
    /// </summary>
    public Task<IEnumerable<Models.ExamMenu>> GetExamMenusAsync();

    /// <summary>
    /// 前提検査メニューの設定一覧を取得します。
    /// </summary>
    public Task<IEnumerable<Models.PriorExamMenu>> GetPriorExamMenusAsync();
}

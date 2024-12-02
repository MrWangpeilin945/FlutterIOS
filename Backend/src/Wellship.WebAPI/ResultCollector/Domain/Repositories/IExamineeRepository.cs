namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

/// <summary>
/// 受診者リポジトリ
/// </summary>
public interface IExamineeRepository
{
    /// <summary>
    /// 受診者IDで受診者を取得します。
    /// </summary>
    /// <param name="examineeId">ログインID</param>
    public Task<Models.Examinee> GetExamineeAsync(int examineeId);
}

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

/// <summary>
/// 職員リポジトリ
/// </summary>
public interface IStaffRepository
{
    /// <summary>
    /// ログインIDで職員を取得します。
    /// </summary>
    /// <param name="loginId">ログインID</param>
    public Task<Models.Staff> GetStaffByLoginIdAsync(string loginId);

    /// <summary>
    /// 職員IDで職員を取得します。
    /// </summary>
    /// <param name="staffId">職員ID</param>
    public Task<Models.Staff> GetStaffByStaffIdAsync(Guid staffId);
}


using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

/// <summary>
/// 職員ログイン履歴リポジトリ
/// </summary>
public interface IStaffLoginHistoryRepository
{
    /// <summary>
    /// 指定した職員がログインに成功したログを書く
    /// </summary>
    public ValueTask WriteLoginSucceededLogAsync(Staff staff);
    /// <summary>
    /// 指定した職員がログインに失敗したログを書く
    /// </summary>
    public ValueTask WriteLoginFailedLogAsync(Staff staff);
}

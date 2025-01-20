using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

/// <summary>
/// アプリケーション設定リポジトリ
/// </summary>
public interface IAppConfigRepository
{
    /// <summary>
    /// すべての設定値を取得します。
    /// </summary>
    Task<AppConfigs> GetAllAsync();
}

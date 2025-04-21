using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

/// <summary>
/// 連携処理結果リポジトリのインターフェース
/// </summary>
public interface IIntegrationResultRepository
{
    /// <summary>
    /// 連携処理結果ログを保存します。
    /// </summary>
    /// <param name="resultLogs">連携処理結果ログリスト</param>
    public Task SaveIntegrationResultLogAsync(IEnumerable<IntegrationResultLogWriteModel> resultLogs);
}

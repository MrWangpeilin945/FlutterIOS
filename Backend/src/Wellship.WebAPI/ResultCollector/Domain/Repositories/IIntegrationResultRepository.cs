using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

/// <summary>
/// 連携処理結果リポジトリのインターフェース
/// </summary>
public interface IIntegrationResultRepository
{
    /// <summary>
    /// 指定されたIDの送信対象ログリストを取得します。
    /// </summary>
    /// <param name="logIds">ログIDリスト</param>
    /// <returns>送信対象ログリスト</returns>
    public Task<IEnumerable<IntegrationResultLog>> GetIntegrationResultLogsAsync(List<Guid> logIds);

    /// <summary>
    /// 連携処理結果ログを保存します。
    /// </summary>
    /// <param name="resultLogs">連携処理結果ログリスト</param>
    public Task SaveIntegrationResultLogAsync(IEnumerable<IntegrationResultLogWriteModel> resultLogs);
}

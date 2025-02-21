using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

/// <summary>
/// ログリポジトリ
/// </summary>
public interface ILogRepository
{
    /// <summary>
    /// ログを書き込みます。
    /// </summary>
    Task WriteLogAsync(AppLog appLog);
}

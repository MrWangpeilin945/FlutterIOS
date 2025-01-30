using Dapper;

using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Auth;
using Ryobi.Wellship.WebAPI.ResultCollector.Utilities;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.RepositoryImpls;

/// <summary>
/// ログリポジトリ
/// </summary>
public class LogRepository : ILogRepository
{
    private readonly IDbConnectionProvider _dbConnectionProvider;

    /// <summary>
    /// リポジトリを生成します。
    /// </summary>
    /// <param name="dbConnectionProvider">dbConnectionProvider</param>
    public LogRepository(IDbConnectionProvider dbConnectionProvider)
    {
        _dbConnectionProvider = dbConnectionProvider;
    }

    /// <inheritdoc/>
    public async Task WriteLogAsync(AppLog appLog)
    {
        // var staffCode = _staffIdentityProvider.StaffCode;

        // テーブル書き込み
        var connection = await _dbConnectionProvider.GetOrOpenAsync();

        const string sql = @"
        insert 
        into resultcollector.app_logs(log_level, message, details, created_by) 
        values (@LogLevel, @Message, @Details::jsonb, @CreatedBy);";

        await connection.ExecuteAsync(sql, new
        {
            LogLevel = appLog.LogLevel.ToNLogLevel().ToString(),
            Message = appLog.Message,
            Details = appLog.DetailsJson,
            CreatedBy = "logger"
        });
    }
}

using Dapper;

using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;


/// <inheritdoc/>
public class StaffLoginHistoryRepository(IDbConnectionProvider dbConnectionProvider, TimeProvider timeProvider) : IStaffLoginHistoryRepository
{
    private readonly IDbConnectionProvider _dbConnectionProvider = dbConnectionProvider;
    private readonly TimeProvider _timeProvider = timeProvider;

    /// <inheritdoc/>
    public async ValueTask WriteLoginSucceededLogAsync(Staff staff) => await WriteLog(staff, true);

    /// <inheritdoc/>
    public async ValueTask WriteLoginFailedLogAsync(Staff staff) => await WriteLog(staff, false);
    /// <summary>
    /// 指定した職員のログイン成功・失敗履歴を書き込みます。
    /// </summary>
    /// <param name="staff">職員</param>
    /// <param name="succeeded">ログイン成否</param>
    /// <returns></returns>
    private async ValueTask WriteLog(Staff staff, bool succeeded)
    {
        var conn = await _dbConnectionProvider.GetOrOpenAsync();
        var query = @"
insert into resultcollector.staff_login_histories
(
    staff_id
    , login_timestamp
    , login_success
    , created_by
)
values
(
    @StaffId
    , @LoginTimeStamp
    , @LoginSuccess
    , @CreatedBy
);";
        var param = new
        {
            StaffId = staff.StaffId,
            LoginTimeStamp = _timeProvider.GetUtcNow(),
            LoginSuccess = succeeded,
            CreatedBy = staff.StaffCode
        };
        await conn.ExecuteAsync(query, param);
    }
}

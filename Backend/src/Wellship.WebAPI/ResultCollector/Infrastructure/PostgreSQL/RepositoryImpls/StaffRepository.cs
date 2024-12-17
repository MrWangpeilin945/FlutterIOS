using Dapper;

using Ryobi.Wellship.Core.Exceptions;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.RepositoryImpls;

/// <summary>
/// 職員リポジトリ
/// </summary>
public class StaffRepository : IStaffRepository
{
    private readonly IDbConnectionProvider _dbConnectionProvider;

    /// <summary>
    /// リポジトリを生成します。
    /// </summary>
    /// <param name="dbConnectionProvider">dbConnectionProvider</param>
    public StaffRepository(IDbConnectionProvider dbConnectionProvider)
    {
        _dbConnectionProvider = dbConnectionProvider;
    }

    /// <summary>
    /// ログインIDで職員を取得します。
    /// </summary>
    /// <param name="loginId">ログインID</param>
    public async Task<Staff> GetStaffByLoginIdAsync(string loginId)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        const string sql = @"
       select
           staff_id as StaffId
           , staff_code as StaffCode
           , login_id as LoginId
           , name as Name
           , enabled as Enabled
           , role_id as RoleId
           , password_hash as PasswordHash
           , password_salt as PasswordSalt 
       from
           resultcollector.staffs 
       where
           login_id = @LoginId;";

        var response = await connection.QueryAsync<StaffEntity>(sql, new { LoginId = loginId });
        var staff = response.SingleOrDefault();

        if (staff is null)
        {
            throw new StaffNotFoundException();
        }

        return new Domain.Models.Staff(staff);
    }

    /// <summary>
    /// 職員IDで職員を取得します。
    /// </summary>
    /// <param name="staffId">職員ID</param>
    public async Task<Staff> GetStaffByStaffIdAsync(Guid staffId)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        const string sql = @"
       select
           staff_id as StaffId
           , staff_code as StaffCode
           , login_id as LoginId
           , name as Name
           , enabled as Enabled
           , role_id as RoleId
           , password_hash as PasswordHash
           , password_salt as PasswordSalt 
       from
           resultcollector.staffs 
       where
           staff_id = @StaffId;";

        var response = await connection.QueryAsync<StaffEntity>(sql, new { StaffId = staffId });
        var staff = response.SingleOrDefault();

        if (staff is null)
        {
            throw new StaffNotFoundException();
        }

        return new Domain.Models.Staff(staff);
    }
}

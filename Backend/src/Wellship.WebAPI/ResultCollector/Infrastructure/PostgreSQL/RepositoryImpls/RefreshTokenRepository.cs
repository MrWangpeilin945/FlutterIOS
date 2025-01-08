using Dapper;

using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.RepositoryImpls;

/// <inheritdoc/>
public class RefreshTokenRepository(IDbConnectionProvider dbConnectionProvider) : IRefreshTokenRepository
{
    private readonly IDbConnectionProvider _dbConnectionProvider = dbConnectionProvider;

    /// <inheritdoc/>
    public async ValueTask<RefreshToken?> GetRefreshTokenOrNullAsync(Guid staffId)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        const string query = @"
        select
            staff_id as StaffId
            , token as Token
            , expires_at as ExpiresAt
        from
            resultcollector.refresh_tokens
        where
            staff_id = @StaffId;";

        var result = await connection.QuerySingleOrDefaultAsync<RefreshTokenEntity>(query, new { StaffId = staffId });
        if (result is null)
        {
            return null;
        }
        return new RefreshToken()
        {
            Token = result.Token,
            ExpiresAt = result.ExpiresAt,
        };
    }

    /// <inheritdoc/>
    public async ValueTask UpdateRefreshTokenAsync(Guid staffId, RefreshToken refreshTokenEntity)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        const string query = @"
        merge into resultcollector.refresh_tokens r
        using (values (@StaffId, @Token, @ExpiresAt)) as new_data(
            staff_id
            , token
            , expires_at)
            on r.staff_id = new_data.staff_id
        when matched then
            update
            set
                token = new_data.token
        when not matched then
            insert (
                staff_id
                , token
                , expires_at
                , created_by
            )
            values (
                new_data.staff_id
                , new_data.token
                , new_data.expires_at
                , 'system'
            );";
        await connection.ExecuteAsync(query, new { StaffId = staffId, refreshTokenEntity.Token, refreshTokenEntity.ExpiresAt });
    }

    /// <inheritdoc/>
    public async ValueTask ExpireRefreshTokenAsync(Guid staffId)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        const string query = @"
        delete
            from resultcollector.refresh_tokens
        where
            staff_id = @StaffId;";
        await connection.ExecuteAsync(query, new { StaffId = staffId });
    }
}

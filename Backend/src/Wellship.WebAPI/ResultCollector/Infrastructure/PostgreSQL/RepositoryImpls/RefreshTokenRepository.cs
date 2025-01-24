using Dapper;

using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.RepositoryImpls;

/// <inheritdoc/>
public class RefreshTokenRepository(IDbConnectionProvider dbConnectionProvider, TimeProvider timeProvider) : IRefreshTokenRepository
{
    private readonly IDbConnectionProvider _dbConnectionProvider = dbConnectionProvider;
    private readonly TimeProvider _timeProvider = timeProvider;

    /// <inheritdoc/>
    public async ValueTask<RefreshToken?> GetRefreshTokenOrNullAsync(Guid staffId, Guid sid)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        const string query = @"
        select
            staff_id as StaffId
            , sid as Sid
            , token as Token
            , expires_at as ExpiresAt
        from
            resultcollector.refresh_tokens
        where
            staff_id = @StaffId
        and sid = @Sid;";

        var result = await connection.QuerySingleOrDefaultAsync<RefreshTokenEntity>(query, new { StaffId = staffId, Sid = sid });
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
    public async ValueTask UpdateRefreshTokenAsync(Guid staffId, Guid sid, RefreshToken refreshTokenEntity)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        const string query = @"
        merge into resultcollector.refresh_tokens r
        using (values (@StaffId, @Sid, @Token, @ExpiresAt, @CreatedAt)) as new_data(
            staff_id
            , sid
            , token
            , expires_at
            , created_at)
            on r.staff_id = new_data.staff_id
           and r.sid = new_data.sid
        when matched then
            update
            set
                token = new_data.token
                , created_at = new_data.created_at
        when not matched then
            insert (
                staff_id
                , sid
                , token
                , expires_at
                , created_by
            )
            values (
                new_data.staff_id
                , new_data.sid
                , new_data.token
                , new_data.expires_at
                , 'system'
            );";
        await connection.ExecuteAsync(query, new
        {
            StaffId = staffId,
            Sid = sid,
            refreshTokenEntity.Token,
            refreshTokenEntity.ExpiresAt,
            CreatedAt = _timeProvider.GetUtcNow(),
        });
    }

    /// <inheritdoc/>
    public async ValueTask ExpireRefreshTokenAsync(Guid staffId, Guid sid)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        const string query = @"
        delete
            from resultcollector.refresh_tokens
        where
            staff_id = @StaffId
        and sid = @Sid;";
        await connection.ExecuteAsync(query, new { StaffId = staffId, Sid = sid });
    }

    /// <inheritdoc/>
    public async ValueTask DeleteOutdatedRefreshTokensAsync(Guid staffId)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        var utcNow = _timeProvider.GetUtcNow();
        const string query = @"
        delete
            from resultcollector.refresh_tokens
        where
            staff_id = @StaffId
        and expires_at < @Now";
        await connection.ExecuteAsync(query, new { StaffId = staffId, Now = utcNow });
    }
}

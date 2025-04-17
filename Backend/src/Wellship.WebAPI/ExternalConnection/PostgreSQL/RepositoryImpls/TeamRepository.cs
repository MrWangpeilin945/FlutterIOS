using Dapper;

using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Transaction;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls
{
    /// <summary>
    /// 班を登録するRepository層
    /// </summary>
    public class TeamRepository : ITeamRepository
    {
        private readonly IDbConnectionProvider _dbConnectionProvider;

        /// <summary>
        /// リポジトリを生成します。
        /// </summary>
        /// <param name="dbConnectionProvider">dbConnectionProvider</param>
        public TeamRepository(IDbConnectionProvider dbConnectionProvider)
        {
            _dbConnectionProvider = dbConnectionProvider;
        }

        /// <summary>
        /// 班を登録するRepository層
        /// </summary>
        /// <param name="teams">班</param>
        /// <param name="createdAt">作成日時</param>
        /// <param name="createdBy">作成者</param>
        public async Task UpsertTeamsAsync(List<TeamEntity> teams, DateTimeOffset createdAt, string createdBy)
        {
            using var scope = TransactionScopeHelper.GetTransactionScope();
            using var connection = await _dbConnectionProvider.GetOrOpenAsync();
            {
                // 表示順を取得する
                const string selectOrderNumberSql = @"
                select 
                    coalesce(max(order_number), 0) + 1
                from 
                    resultcollector.teams";
                var result = await connection.QueryAsync<int>(selectOrderNumberSql);
                int orderNumber = result.FirstOrDefault();

                var upsertTeams = teams.Select((x, index) => new
                {
                    TeamId = Guid.NewGuid(),
                    TeamCode = x.TeamCode,
                    Name = x.Name,
                    OrderNumber = orderNumber + index,
                    CreatedAt = createdAt,
                    CreatedBy = createdBy,
                }).ToArray();

                // teams（班）
                const string mergeTeamsSql = @"
                merge
                into resultcollector.teams as team
                    using (values (@TeamId, @TeamCode, @Name, @OrderNumber,
                                    @CreatedAt, @CreatedBy)) as new_data(
                        team_id
                        , team_code
                        , name
                        , order_number
                        , created_at
                        , created_by
                    )
                    on team.team_code = new_data.team_code
                when matched then 
                    update set
                        name = new_data.name
                        , created_at = new_data.created_at
                        , created_by = new_data.created_by 
                when not matched then
                    insert (
                        team_id
                        , team_code
                        , name
                        , order_number
                        , created_at
                        , created_by
                    )
                    values (
                        new_data.team_id
                        , new_data.team_code
                        , new_data.name
                        , new_data.order_number
                        , new_data.created_at
                        , new_data.created_by
                    );";
                await connection.ExecuteAsync(mergeTeamsSql, upsertTeams);
                scope.Complete();
            }
        }

        /// <summary>
        /// 存在する班コードを取得する。
        /// </summary>
        /// <param name="teamCodes">班コードリスト</param>
        /// <returns>存在する班コードリスト</returns>
        public async Task<List<string>> GetTeamsByCodesAsync(List<string> teamCodes)
        {
            var connection = await _dbConnectionProvider.GetOrOpenAsync();
            var sql = @"
                    select
                        team_code
                    from
                        resultcollector.teams
                    where
                        team_code = any(@TeamCodes);";

            var result = await connection.QueryAsync<string>(sql, new { TeamCodes = teamCodes.ToArray() });
            return result.ToList();
        }

        /// <summary>
        /// 班情報を取得する
        /// </summary>
        /// <param name="teamCodes">班コード</param>
        /// <returns></returns>
        public async Task<List<TeamEntity>> GetTeamInfoAsync(List<string> teamCodes)
        {
            var connection = await _dbConnectionProvider.GetOrOpenAsync();
            var sql = @"
                    select
                        team_id as TeamId, team_code as TeamCode
                    from
                        resultcollector.teams
                    where
                        team_code = any(@TeamCodes);";

            var result = await connection.QueryAsync<TeamEntity>(sql, new { TeamCodes = teamCodes.ToArray() });

            return result.ToList();
        }
    }
}
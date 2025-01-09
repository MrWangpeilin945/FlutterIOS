using Dapper;

using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;
using Ryobi.Wellship.WebAPI.ExternalConnection.Utilities;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Helper;
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
        public async Task UpsertTeamsAsync(List<TeamEntity> teams, DateTime createdAt, string createdBy)
        {
            using var scope = TransactionScopeHelper.GetTransactionScope();
            {
                using var connection = await _dbConnectionProvider.GetOrOpenAsync();
                {
                    // 一時テーブル作成
                    string sqlCreateTempTable = @"
                        create temp table tmp_teams(
                            team_code text not null
                            , name text not null
                        ) on commit drop;";
                    // 一時テーブル作成 SQL実行
                    await connection.ExecuteAsync(sqlCreateTempTable);

                    // 一時テーブルにINSERT
                    await BulkInsertHelper.BulkInsert(teams,
                        team =>
                        $"(" +
                        $"{SqlFormatter.EscapeSqlValue(team.TeamCode)}, " +
                        $"{SqlFormatter.EscapeSqlValue(team.Name)}" +
                        $")",
                        "tmp_teams",
                        connection);


                    // UPSERT処理
                    string upsertSql = @"   
                        with max_order_number as (
                            select coalesce(max(order_number), 0) as max_number from resultcollector.teams
                        )
                        insert into resultcollector.teams (team_code, name, order_number, created_at, created_by)
                        select
                            team_code,
                            name,
                            max_order.max_number + row_number() over(),
                            @CreatedAt,
                            @CreatedBy
                        from tmp_teams
                        cross join max_order_number as max_order
                        on conflict (team_code)
                        do update set
                            name = excluded.name,
                            created_at = excluded.created_at,
                            created_by = excluded.created_by;
                    ";
                    // UPSERT処理 SQL実行
                    await connection.ExecuteAsync(upsertSql, new { CreatedAt = createdAt, CreatedBy = createdBy });
                }

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
        public async Task<List<TeamInfoEntity>> GetTeamInfoAsync(List<string> teamCodes)
        {
            var connection = await _dbConnectionProvider.GetOrOpenAsync();
            var sql = @"
                    select
                        team_id as TeamId, team_code as TeamCode
                    from
                        resultcollector.teams
                    where
                        team_code = any(@TeamCodes);";

            var result = await connection.QueryAsync<TeamInfoEntity>(sql, new { TeamCodes = teamCodes.ToArray() });

            return result.ToList();
        }
    }
}
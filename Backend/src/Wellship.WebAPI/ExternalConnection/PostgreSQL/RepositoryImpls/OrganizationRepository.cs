using Dapper;

using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Helper;
using Ryobi.Wellship.WebAPI.ExternalConnection.Utilities;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Transaction;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls
{
    /// <summary>
    /// 団体リポジトリ
    /// </summary>
    public class OrganizationRepository : IOrganizationRepository
    {
        private readonly IDbConnectionProvider _dbConnectionProvider;

        /// <summary>
        /// リポジトリを生成します。
        /// </summary>
        /// <param name="dbConnectionProvider">dbConnectionProvider</param>
        public OrganizationRepository(IDbConnectionProvider dbConnectionProvider)
        {
            _dbConnectionProvider = dbConnectionProvider;
        }

        /// <summary>
        /// 団体を登録する。
        /// </summary>
        /// <param name="organizations">団体エンティティリスト</param>
        public async Task UpsertOrganaizationsAsync(List<OrganizationEntity> organizations)
        {
            using var scope = TransactionScopeHelper.GetTransactionScope();
            try
            {
                using var connection = await _dbConnectionProvider.GetOrOpenAsync();
                {
                    // 一時テーブル作成
                    string tempTableCreateSql = @"
                        create temp table temp_organizations (
                            organization_code text not null,
                            name text not null,
                            created_at timestamp(6) with time zone not null,
                            created_by text not null
                        ) on commit drop;
                    ";
                    await connection.ExecuteAsync(tempTableCreateSql);

                    // 一時テーブルへの挿入
                    await BulkInsertHelper.BulkInsert(organizations,
                        organization =>
                        $"({SqlFormatter.EscapeSqlValue(organization.OrganizationCode)}, " +
                        $"{SqlFormatter.EscapeSqlValue(organization.Name)}, " +
                        $"{SqlFormatter.EscapeSqlValue(organization.CreatedAt)}, " +
                        $"{SqlFormatter.EscapeSqlValue(organization.CreatedBy)})",
                        "temp_organizations",
                        connection);

                    // アップサート処理
                    string upsertSql = @"   
                        with max_order_number as (
                            select coalesce(max(order_number), 0) as max_number from resultcollector.organizations
                        )
                        insert into resultcollector.organizations (organization_code, name, order_number, created_at, created_by)
                        select
                            organization_code,
                            name,
                            max_order.max_number + row_number() over(),
                            created_at,
                            created_by
                        from temp_organizations
                        cross join max_order_number as max_order
                        on conflict (organization_code)
                        do update set
                            name = excluded.name,
                            created_at = excluded.created_at,
                            created_by = excluded.created_by;
                    ";
                    await connection.ExecuteAsync(upsertSql);
                }

                scope.Complete();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

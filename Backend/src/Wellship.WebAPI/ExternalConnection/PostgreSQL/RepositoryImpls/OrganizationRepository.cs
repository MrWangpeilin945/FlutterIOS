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
        /// <param name="createdAt">作成日時</param>
        /// <param name="createdBy">作成者</param>
        public async Task UpsertOrganizationsAsync(List<OrganizationEntity> organizations, DateTimeOffset createdAt, string createdBy)
        {
            using var scope = TransactionScopeHelper.GetTransactionScope();
            {
                using var connection = await _dbConnectionProvider.GetOrOpenAsync();
                {
                    // 表示順を取得する
                    const string selectOrderNumberSql = @"
                    select 
                        coalesce(max(order_number), 0) + 1
                    from 
                        resultcollector.organizations";
                    var result = await connection.QueryAsync<int>(selectOrderNumberSql);
                    int orderNumber = result.FirstOrDefault();

                    var upsertOrganizations = organizations.Select((x, index) => new
                    {
                        OrganizationId = Guid.NewGuid(),
                        OrganizationCode = x.OrganizationCode,
                        Name = x.Name,
                        OrderNumber = orderNumber + index,
                        CreatedAt = createdAt,
                        CreatedBy = createdBy,
                    }).ToArray();
                    // organizations（団体）
                    const string mergeOrganizationsSql = @"
                    merge
                    into resultcollector.organizations as organiza
                        using (values (@OrganizationId, @OrganizationCode, @Name, @OrderNumber,
                                       @CreatedAt, @CreatedBy)) as new_data(
                            organization_id
                            , organization_code
                            , name
                            , order_number
                            , created_at
                            , created_by
                        )
                        on organiza.organization_code = new_data.organization_code
                    when matched then 
                        update set
                            name = new_data.name
                            , created_at = new_data.created_at
                            , created_by = new_data.created_by 
                    when not matched then
                        insert (
                            organization_id
                            , organization_code
                            , name
                            , order_number
                            , created_at
                            , created_by
                        )
                        values (
                            new_data.organization_id
                            , new_data.organization_code
                            , new_data.name
                            , new_data.order_number
                            , new_data.created_at
                            , new_data.created_by
                        );";
                    await connection.ExecuteAsync(mergeOrganizationsSql, upsertOrganizations);
                }
                scope.Complete();
            }
        }

        /// <summary>
        /// 存在する団体情報を取得する
        /// </summary>
        /// <param name="organizationCodes">団体コードリスト</param>
        /// <returns></returns>
        public async Task<List<OrganizationEntity>> GetOrganizationInfoAsync(List<string> organizationCodes)
        {
            var connection = await _dbConnectionProvider.GetOrOpenAsync();
            var sql = @"
                    select
                        organization_id as OrganizationId, 
                        organization_code as OrganizationCode,
                        name as Name
                    from
                        resultcollector.organizations
                    where
                        organization_code = any(@OrganizationCodes);";

            var result = await connection.QueryAsync<OrganizationEntity>(sql, new { OrganizationCodes = organizationCodes });

            return result.ToList();
        }
    }
}

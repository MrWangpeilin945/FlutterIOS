using System.Transactions;

using Dapper;

using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Helper;
using Ryobi.Wellship.WebAPI.ExternalConnection.Utilities;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Transaction;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls
{
    /// <summary>
    /// 所属リポジトリ
    /// </summary>
    public class AffiliationRepository : IAffiliationRepository
    {
        private readonly IDbConnectionProvider _dbConnectionProvider;

        /// <summary>
        /// リポジトリを生成します。
        /// </summary>
        /// <param name="dbConnectionProvider">dbConnectionProvider</param>
        public AffiliationRepository(IDbConnectionProvider dbConnectionProvider)
        {
            _dbConnectionProvider = dbConnectionProvider;
        }

        /// <summary>
        /// 所属を登録する。
        /// </summary>
        /// <param name="examinees">受診者リスト</param>
        /// <param name="createAt">作成日時</param>
        /// <param name="createdBy">作成者</param>
        /// <returns></returns>
        public async Task InsertAffiliationsAsync(List<Examinee> examinees, DateTimeOffset createAt, string createdBy)
        {
            using var connection = await _dbConnectionProvider.GetOrOpenAsync();
            {
                using var scope = TransactionScopeHelper.GetTransactionScope();
                {
                    // トランザクションを接続に登録
                    connection.EnlistTransaction(Transaction.Current);

                    // 受診者コードと団体コードのペアリストを生成
                    var examineeOrganizationPairs = examinees
                        .SelectMany(examinee => examinee.Affiliations
                        .Select(affiliation => new { examinee.ExamineeCode, affiliation.OrganizationCode }))
                        .ToList();

                    // 一括で受診者IDと団体IDを取得
                    var sql = @"
                        select 
                            e.examinee_id as ExamineeId,
                            e.examinee_code as ExamineeCode,
                            o.organization_id as OrganizationId,
                            o.organization_code as OrganizationCode
                        from 
                            resultcollector.examinees e
                        join 
                            resultcollector.organizations o 
                        on 
                            o.organization_code = any(@OrganizationCodes)
                        where 
                            e.examinee_code = any(@ExamineeCodes);";

                    // 全ての受診者IDと団体IDを取得
                    var results = await connection.QueryAsync<AffiliationEntity>(sql, new
                    {
                        ExamineeCodes = examineeOrganizationPairs.Select(x => x.ExamineeCode).Distinct().ToArray(),
                        OrganizationCodes = examineeOrganizationPairs.Select(x => x.OrganizationCode).Distinct().ToArray()
                    });

                    // 結果をフィルタリング後、所属エンティティリスト生成
                    var affiliationEntities = new List<AffiliationEntity>();
                    foreach (var examinee in examinees)
                    {
                        var examineeCode = examinee.ExamineeCode;
                        var organizationCodes = examinee.Affiliations.Select(x => x.OrganizationCode).ToList();
                        var matchedResults = results
                            .Where(x => x.ExamineeCode == examineeCode && organizationCodes.Contains(x.OrganizationCode));

                        affiliationEntities.AddRange(matchedResults);
                    }

                    // 所属テーブルの重複データを削除
                    var deleteSql = @"
                        delete from 
                            resultcollector.affiliations 
                        where 
                            examinee_id = any(@ExamineeIds);";
                    await connection.ExecuteAsync(deleteSql, new { ExamineeIds = affiliationEntities.Select(x => x.ExamineeId).ToArray() });

                    // 所属テーブルへの登録
                    await BulkInsertHelper.BulkInsert(affiliationEntities,
                        affiliation =>
                        $"(" +
                        $"{SqlFormatter.EscapeSqlValue(affiliation.ExamineeId)}," +
                        $"{SqlFormatter.EscapeSqlValue(affiliation.OrganizationId)}," +
                        $"{SqlFormatter.EscapeSqlValue(createAt)}," +
                        $"{SqlFormatter.EscapeSqlValue(createdBy)}" +
                        $")",
                        "resultcollector.affiliations",
                        connection);

                    // コミット
                    scope.Complete();
                }
            }
        }
    }
}

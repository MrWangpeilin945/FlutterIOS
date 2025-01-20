using Dapper;

using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls
{
    /// <summary>
    /// 外部検査項目明細リポジトリ
    /// </summary>
    public class ExternalExamItemDetailsRepository : IExternalExamItemDetailsRepository
    {
        private readonly IDbConnectionProvider _dbConnectionProvider;

        /// <summary>
        /// リポジトリを生成します。
        /// </summary>
        /// <param name="dbConnectionProvider">dbConnectionProvider</param>
        public ExternalExamItemDetailsRepository(IDbConnectionProvider dbConnectionProvider)
        {
            _dbConnectionProvider = dbConnectionProvider;
        }

        /// <summary>
        /// 存在する外部コード検査項目明細CDを取得する
        /// </summary>
        /// <param name="externalExamItemDetailCodes"></param>
        /// <returns></returns>
        public async Task<List<ExternalExamItemDetailEntity>> GetDetailsByCodesAsync(List<string> externalExamItemDetailCodes)
        {
            var connection = await _dbConnectionProvider.GetOrOpenAsync();
            var sql = @"
                    select
                        exam_item_detail_id as ExamItemDetailId, external_exam_item_detail_code as ExternalExamItemDetailCode
                    from
                        resultcollector.external_exam_item_details
                    where
                        external_exam_item_detail_code = any(@ExternalExamItemDetailCodes);";

            var result = await connection.QueryAsync<ExternalExamItemDetailEntity>(sql, new { ExternalExamItemDetailCodes = externalExamItemDetailCodes.ToArray() });
            return result.ToList();
        }
    }
}

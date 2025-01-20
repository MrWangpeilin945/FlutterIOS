using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls
{
    /// <summary>
    /// 外部検査項目明細リポジトリインターフェース
    /// </summary>
    public interface IExternalExamItemDetailsRepository
    {
        /// <summary>
        /// 存在する外部コード検査項目明細CDを取得する
        /// </summary>
        /// <param name="externalExamItemDetailCodes"></param>
        /// <returns></returns>
        public Task<List<ExternalExamItemDetailEntity>> GetDetailsByCodesAsync(List<string> externalExamItemDetailCodes);
    }
}

using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls
{
    /// <summary>
    /// 所属リポジトリインターフェース
    /// </summary>
    public interface IAffiliationRepository
    {
        /// <summary>
        /// 所属を登録する。
        /// </summary>
        /// <param name="examinees">受診者リスト</param>
        /// <param name="createAt">作成日時</param>
        /// <param name="createdBy">作成者</param>
        /// <returns></returns>
        public Task InsertAffiliationsAsync(List<Examinee> examinees, DateTime createAt, string createdBy);
    }
}

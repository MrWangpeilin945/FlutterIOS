using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls
{
    /// <summary>
    /// 団体リポジトリインターフェース
    /// </summary>
    public interface IOrganizationRepository
    {
        /// <summary>
        /// 団体を登録する。
        /// </summary>
        /// <param name="organizations">団体エンティティリスト</param>
        /// <param name="createdAt">作成日時</param>
        /// <param name="createdBy">作成者</param>
        public Task UpsertOrganizationsAsync(List<OrganizationEntity> organizations, DateTimeOffset createdAt, string createdBy);

        /// <summary>
        /// 存在する団体コードを取得する
        /// </summary>
        /// <param name="organizationCodes">団体コードリスト</param>
        /// <returns>存在する団体コードリスト</returns>
        public Task<List<string>> GetOrganizationsByCodesAsync(List<string> organizationCodes);
    }
}

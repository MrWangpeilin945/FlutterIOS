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
        public Task UpsertOrganaizationsAsync(List<OrganizationEntity> organizations);
    }
}

using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls
{
    /// <summary>
    /// 職員リポジトリインターフェース
    /// </summary>
    public interface IStaffRepository
    {
        /// <summary>
        /// 職員を登録する
        /// </summary>
        /// <param name="staffEntities"></param>
        /// <param name="createdAt"></param>
        /// <param name="createdBy"></param>
        /// <returns></returns>
        public Task UpsertStaffAsync(List<StaffEntity> staffEntities, DateTimeOffset createdAt, string createdBy);
        /// <summary>
        /// 存在する職員情報を取得する
        /// </summary>
        /// <param name="loginIds">ログインID</param>
        /// <returns></returns>
        public Task<List<StaffEntity>> GetStaffsInfoByLoginIdsAsync(List<string> loginIds);

    }
}

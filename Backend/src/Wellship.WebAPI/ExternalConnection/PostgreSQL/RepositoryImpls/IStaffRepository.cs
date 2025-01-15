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
        public Task UpsertStaffAsync(List<StaffEntity> staffEntities, DateTime createdAt, string createdBy);
        /// <summary>
        /// 職員コードとログインIDのペアを取得する
        /// </summary>
        /// <param name="staffCodesAndLoginIds"></param>
        /// <returns></returns>
        public Task<List<(string staffCode, string loginId)>> GetStaffsByLoginIdsAsync(List<(string staffCode, string loginId)> staffCodesAndLoginIds);
    }
}

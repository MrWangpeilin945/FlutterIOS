using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls
{
    /// <summary>
    /// 会場日時リポジトリインターフェース
    /// </summary>
    public interface IPlaceScheduleRepository
    {
        /// <summary>
        /// 会場日時を登録する
        /// </summary>
        /// <param name="placeScheduleEntities"></param>
        /// <param name="createdAt"></param>
        /// <param name="createdBy"></param>
        /// <returns></returns>
        public Task UpsertPlaceScheduleAsync(List<PlaceScheduleEntity> placeScheduleEntities, DateTime createdAt, string createdBy);
    }
}

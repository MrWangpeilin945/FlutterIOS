using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls
{
    /// <summary>
    /// 会場を登録するRepository層
    /// </summary>
    public interface IPlaceRepository
    {
        /// <summary>
        /// 会場を登録するRepository層
        /// </summary>
        /// <param name="places">会場</param>
        /// <param name="createdAt">作成日時</param>
        /// <param name="createdBy">作成者</param>
        public Task UpsertPlacesAsync(List<PlaceEntity> places, DateTime createdAt, string createdBy);

    }
}

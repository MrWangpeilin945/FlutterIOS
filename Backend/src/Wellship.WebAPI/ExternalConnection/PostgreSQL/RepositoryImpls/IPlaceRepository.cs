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
        public Task UpsertPlacesAsync(List<PlaceEntity> places, DateTimeOffset createdAt, string createdBy);

        /// <summary>
        /// 存在する会場コードを取得する
        /// </summary>
        /// <param name="placeCodes">会場コードリスト</param>
        /// <returns>存在する会場コードリスト</returns>
        public Task<List<string>> GetPlacesByCodesAsync(List<string> placeCodes);

        /// <summary>
        /// 存在する会場情報（ID、コード）を取得する
        /// </summary>
        /// <param name="placeCodes"></param>
        /// <returns></returns>
        public Task<List<PlaceEntity>> GetPlaceInfoAsync(List<string> placeCodes);

    }
}

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

        /// <summary>
        /// 存在する会場日程（会場ID、会場コード、班ID、班コード、健診日）を取得する
        /// </summary>
        /// <param name="placeCodes">会場コードのリスト</param>
        /// <param name="teamCodes">班コードのリスト</param>
        /// <param name="examDates">健診日のリスト</param>
        public Task<List<PlaceScheduleEntity>> GetPlaceScheduleInfoAsync(List<string> placeCodes, List<string> teamCodes, List<DateOnly> examDates);
    }
}

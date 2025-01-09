using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Usecases
{
    /// <summary>
    /// 会場日程ユースケースインターフェース
    /// </summary>
    public interface IPlaceScheduleUsecase
    {
        /// <summary>
        /// 会場日程を登録する
        /// </summary>
        /// <param name="placeSchedules"></param>
        /// <returns></returns>
        public Task<List<ErrorObject>> StorePlaceSchedulesAsync(List<PlaceSchedule> placeSchedules);
    }
}

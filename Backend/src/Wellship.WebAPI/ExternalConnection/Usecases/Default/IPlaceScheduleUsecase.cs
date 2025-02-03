using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;
/// <summary>
/// EC2012_会場日程を登録する インターフェース
/// </summary>
public interface IPlaceScheduleUsecase
{
    /// <summary>
    /// EC2012_会場日程を登録する
    /// </summary>
    /// <param name="placeSchedules"></param>
    /// <returns></returns>
    public Task<List<ErrorObject>> StorePlaceSchedulesAsync(List<PlaceSchedule> placeSchedules);
}

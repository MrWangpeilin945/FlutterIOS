using Ryobi.WELLSHIP.APIModels.Responses;

namespace Ryobi.WELLSHIP.WebAPI.ResultCollector.Usecases;

/// <summary>
/// 会場日程ユースケースのインターフェース
/// </summary>
public interface IPlaceScheduleUsecase
{
    public IEnumerable<PlaceSchedule> GetList(string? date, int? teamId, int? placeId);
}

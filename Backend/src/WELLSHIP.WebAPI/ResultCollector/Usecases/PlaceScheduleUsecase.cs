using Ryobi.WELLSHIP.APIModels.Responses;

namespace Ryobi.WELLSHIP.WebAPI.ResultCollector.Usecases;

public class PlaceScheduleUsecase : IPlaceScheduleUsecase
{
    public PlaceScheduleUsecase()
    {
        // TODO: DIするものはここから
    }

    public IEnumerable<PlaceSchedule> GetList(string? date, int? teamId, int? placeId)
    {
        // TODO: リポジトリを使ってデータアクセスする
        return
        [
            new(1, new DateOnly(2024, 09, 18), 1, "会場A", 3, "3班", "0900", "1200"),
            new(2, new DateOnly(2024, 09, 18), 1, "会場A", 3, "3班", "1300", "1700"),
            new(3, new DateOnly(2024, 09, 20), 3, "会場C", 5, "5班", "0900", "1200")
        ];
    }
}

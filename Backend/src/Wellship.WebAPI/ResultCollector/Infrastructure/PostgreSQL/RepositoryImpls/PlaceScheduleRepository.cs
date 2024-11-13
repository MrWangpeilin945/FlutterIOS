using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.RepositoryImpls;

/// <summary>
/// 会場日程リポジトリ
/// </summary>
public class PlaceScheduleRepository : IPlaceScheduleRepository
{
    private readonly IDbConnectionProvider _dbConnectionProvider;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public PlaceScheduleRepository(IDbConnectionProvider dbConnectionProvider)
    {
        _dbConnectionProvider = dbConnectionProvider;
    }

    /// <summary>
    /// 健診日を指定して会場日程を取得する
    /// </summary>
    public IEnumerable<PlaceSchedule> GetPlaceSchedules(DateOnly date)
    {
        // TODO: SQLで取得する
        var dummy = new List<PlaceSchedule>()
        {
            new PlaceSchedule(1, new Place(10, "両備システムズ豊成オフィス"), new Team(2, "2班"), new DateOnly(2024, 09, 27), "09:00", "12:00"),
            new PlaceSchedule(2, new Place(10, "両備システムズ豊成オフィス"), new Team(2, "2班"), new DateOnly(2024, 12, 1), "09:00", "12:00"),
            new PlaceSchedule(3, new Place(10, "両備システムズ豊成オフィス"), new Team(3, "3班"), new DateOnly(2024, 09, 27), "09:00", "12:00"),
            new PlaceSchedule(4, new Place(20, "両備システムズ藤崎オフィス"), new Team(2, "2班"), new DateOnly(2024, 09, 27), "13:00", "17:00")
        };
        return dummy.Where(x => x.Date == date);
    }
}

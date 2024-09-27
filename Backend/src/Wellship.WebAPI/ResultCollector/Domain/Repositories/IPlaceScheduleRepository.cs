using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

/// <summary>
/// 会場日程リポジトリ
/// </summary>
public interface IPlaceScheduleRepository
{
    /// <summary>
    /// 健診日を指定して会場日程を取得する
    /// </summary>
    public IEnumerable<PlaceSchedule> GetPlaceSchedules(DateOnly date);
}

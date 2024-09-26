namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 会場日程のレスポンスモデル
/// </summary>
/// <param name="PlaceScheduleId"></param>
/// <param name="Date"></param>
/// <param name="PlaceName"></param>
/// <param name="PlaceId"></param>
/// <param name="TeamId"></param>
/// <param name="TeamName"></param>
/// <param name="StartTime"></param>
/// <param name="EndTime"></param>
public record class PlaceSchedule(int PlaceScheduleId, DateOnly Date, int PlaceId, string PlaceName, int TeamId, string TeamName, string StartTime, string EndTime);

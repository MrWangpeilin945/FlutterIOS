namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;

/// <summary>
/// 会場
/// </summary>
public class Place
{
    /// <summary>
    /// 会場ID
    /// </summary>
    public int PlaceId { get; set; }
    /// <summary>
    /// 会場名
    /// </summary>
    public required string Name { get; set; }
}

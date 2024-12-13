namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;

/// <summary>
/// 班
/// </summary>
public class Team
{
    /// <summary>
    /// 班ID
    /// </summary>
    public Guid TeamId { get; set; }
    /// <summary>
    /// 班名
    /// </summary>
    public required string Name { get; set; }
}

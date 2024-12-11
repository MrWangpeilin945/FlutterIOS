namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 班
/// </summary>
public class Team
{
    /// <summary>
    /// 班ID
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// 班コード
    /// </summary>
    public required string Code { get; init; }

    /// <summary>
    /// 班名
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// 表示順
    /// </summary>
    public required int OrderNumber { get; init; }
}

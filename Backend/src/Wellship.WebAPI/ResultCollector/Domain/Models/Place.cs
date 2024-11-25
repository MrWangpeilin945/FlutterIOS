namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 会場
/// </summary>
public class Place
{
    /// <summary>
    /// 会場ID
    /// </summary>
    public required int Id { get; init; }

    /// <summary>
    /// 会場コード
    /// </summary>
    public required string Code { get; init; }

    /// <summary>
    /// 会場名
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// 表示順
    /// </summary>
    public required int OrderNumber { get; init; }
}

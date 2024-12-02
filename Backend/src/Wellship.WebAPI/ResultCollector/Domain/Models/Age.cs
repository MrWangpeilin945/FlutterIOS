namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 年齢・月齢・日齢
/// </summary>
public class Age
{
    /// <summary>
    /// 年齢
    /// </summary>
    public required int Years { get; init; }

    /// <summary>
    /// 月齢
    /// </summary>
    public required int Months { get; init; }

    /// <summary>
    /// 日齢
    /// </summary>
    public required int Days { get; init; }
}

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 所属団体
/// </summary>
public class Affiliations
{
    /// <summary>
    /// 団体ID
    /// </summary>
    public required int OrganizationId { get; init; }

    /// <summary>
    /// 団体コード
    /// </summary>
    public required string OrganizationCode { get; init; }

    /// <summary>
    /// 団体名
    /// </summary>
    public required string OrganizationName { get; init; }

    /// <summary>
    /// 表示順
    /// </summary>
    public required int OrderNumber { get; init; }

}
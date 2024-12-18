using Ryobi.Wellship.Core.Enums;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 受診者
/// </summary>
public class Examinee
{
    /// <summary>
    /// 受診者ID
    /// </summary>
    public required Guid ExamineeId { get; init; }

    /// <summary>
    /// 受診者コード
    /// </summary>
    public required string ExamineeCode { get; init; }

    /// <summary>
    /// 氏名
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// カナ氏名
    /// </summary>
    public required string KanaName { get; init; }

    /// <summary>
    /// 性別
    /// </summary>
    public required Sex Sex { get; init; }

    /// <summary>
    /// 生年月日
    /// </summary>
    public required Birthdate Birthdate { get; init; }

    /// <summary>
    /// 所属団体
    /// </summary>
    public required IEnumerable<Affiliations> Affiliations { get; init; }
}

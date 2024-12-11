namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;

/// <summary>
/// 受診者のエンティティ
/// </summary>
public class ExamineeEntity
{
    /// <summary>
    /// 受診者ID
    /// </summary>
    public required int ExamineeId { get; init; }

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
    public required int Sex { get; init; }

    /// <summary>
    /// 生年月日
    /// </summary>
    public required DateTime Birthdate { get; init; }

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

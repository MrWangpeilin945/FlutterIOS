namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;

/// <summary>
/// 検査メニュー特記_受診特記のエンティティ
/// </summary>
public class MenuNoteConsultEntity
{
    /// <summary>
    /// 検査メニュー特記ID
    /// </summary>
    public int MenuNoteId { get; init; }

    /// <summary>
    /// 検査特記コード
    /// </summary>
    public required string Code { get; init; }

    /// <summary>
    /// 表示順
    /// </summary>
    public required int OrderNumber { get; init; }
}

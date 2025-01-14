using Ryobi.Wellship.ExternalConnection.Enums;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;
/// <summary>
/// 受付
/// </summary>
public class TicketEntity
{
    /// <summary>
    /// 受診ID
    /// </summary>
    public required Guid ConsultId { get; init; }

    /// <summary>
    /// 受付番号
    /// </summary>
    public required string TicketNumber { get; init; }

    /// <summary>
    /// 連携キー
    /// </summary>
    public required string ConnectionCode { get; init; }

    /// <summary>
    /// 操作区分
    /// </summary>
    public required ActionType ActionType { get; init; }

    /// <summary>
    /// 登録順
    /// </summary>
    public required int OrderNumber { get; init; }
}

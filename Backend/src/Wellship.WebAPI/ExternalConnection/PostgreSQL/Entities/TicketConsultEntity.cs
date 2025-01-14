namespace Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;
/// <summary>
/// 受付対象の受診
/// </summary>
public class TicketConsultEntity
{
    /// <summary>
    /// 受診ID
    /// </summary>
    public required Guid ConsultId { get; init; }

    /// <summary>
    /// 外部連携キー
    /// </summary>
    public required string ConnectionCode { get; init; }
}

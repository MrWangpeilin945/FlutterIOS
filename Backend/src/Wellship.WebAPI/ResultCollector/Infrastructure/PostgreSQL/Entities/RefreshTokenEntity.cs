namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;

/// <summary>
/// リフレッシュトークン
/// </summary>
public class RefreshTokenEntity
{
    /// <summary>
    /// 職員ID
    /// </summary>
    public required Guid StaffId { get; set; }
    /// <summary>
    /// リフレッシュトークン
    /// </summary>
    public required string Token { get; set; }
    /// <summary>
    /// リフレッシュトークン有効期限
    /// </summary>
    public required DateTimeOffset ExpiresAt { get; set; }
}

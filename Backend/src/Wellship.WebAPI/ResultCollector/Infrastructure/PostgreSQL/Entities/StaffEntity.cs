namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;

/// <summary>
/// 職員のエンティティ
/// </summary>
public class StaffEntity
{
    /// <summary>
    /// 職員ID
    /// </summary>
    public Guid StaffId { get; set; }

    /// <summary>
    /// 職員コード
    /// </summary>
    public string StaffCode { get; set; } = null!;

    /// <summary>
    /// ログインID
    /// </summary>
    public string LoginId { get; set; } = null!;

    /// <summary>
    /// 職員名
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// 有効か
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// ロールID
    /// </summary>
    public int RoleId { get; set; }

    /// <summary>
    /// パスワードハッシュ
    /// </summary>
    public byte[] PasswordHash { get; set; } = null!;

    /// <summary>
    /// パスワードソルト
    /// </summary>
    public byte[] PasswordSalt { get; set; } = null!;
}

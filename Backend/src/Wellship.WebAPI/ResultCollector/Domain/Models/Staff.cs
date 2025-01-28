using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 職員
/// </summary>
public class Staff
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Staff(StaffEntity entity)
    {
        StaffId = entity.StaffId;
        StaffCode = entity.StaffCode;
        LoginId = entity.LoginId;
        Name = entity.Name;
        Enabled = entity.Enabled;
        Role = (Role)entity.RoleId;
        Password = new Password(entity.PasswordHash, entity.PasswordSalt);
    }

    /// <summary>
    /// 職員ID
    /// </summary>
    public Guid StaffId { get; }

    /// <summary>
    /// 職員コード
    /// </summary>
    public string StaffCode { get; }

    /// <summary>
    /// ログインID
    /// </summary>
    public string LoginId { get; }

    /// <summary>
    /// 職員名
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 有効か
    /// </summary>
    public bool Enabled { get; }

    /// <summary>
    /// ロール
    /// </summary>
    public Role Role { get; }

    /// <summary>
    /// パスワード認証情報
    /// </summary>
    public Password Password { get; }
}

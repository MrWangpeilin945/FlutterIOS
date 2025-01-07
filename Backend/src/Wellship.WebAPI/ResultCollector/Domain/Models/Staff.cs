using System.Security.Cryptography;
using System.Text;

using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 職員
/// </summary>
public class Staff
{
    private readonly byte[] _passwordHash;
    private readonly byte[] _passwordSalt;

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
        _passwordHash = entity.PasswordHash;
        _passwordSalt = entity.PasswordSalt;
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
    /// パスワードを検証する
    /// </summary>
    /// <param name="password">検証するパスワード</param>
    public bool VerifyPassword(string password)
    {
        if (password == "" || _passwordHash.Length != 64 || _passwordSalt.Length != 128)
        {
            return false;
        }
        using var hmac = new HMACSHA512(_passwordSalt);
        // NOTE: ストレッチングの要否は要件を確認した上で判断します
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return hash.SequenceEqual(_passwordHash);
    }
}

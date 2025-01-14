using System.IdentityModel.Tokens.Jwt;

using Ryobi.Wellship.Core.Enums;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Auth;

/// <summary>
/// ログインしている職員の情報を提供するProviderです。
/// </summary>
public interface IStaffIdentityProvider
{
    /// <summary>
    /// 職員ID
    /// </summary>
    /// <remarks>認証していない場合はnullを返します。</remarks>
    public Guid? StaffId { get; }
    /// <summary>
    /// 職員コード
    /// </summary>
    /// <remarks>認証していない場合はnullを返します。</remarks>
    public string? StaffCode { get; }
    /// <summary>
    /// 権限
    /// </summary>
    /// <remarks>認証していない場合はnullを返します。</remarks>
    public Role? Role { get; }
}

/// <summary>
/// 認証している職員の情報をHttpContextから取得・提供します
/// </summary>
public class StaffIdentityFromHttpContextProvider : IStaffIdentityProvider
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="httpContextAccessor">HttpContextから取得します</param>
    public StaffIdentityFromHttpContextProvider(IHttpContextAccessor httpContextAccessor)
    {
        var user = httpContextAccessor.HttpContext?.User;

        StaffId = Guid.TryParse(user?.Identity?.Name, out var staffId) ? staffId : null;
        StaffCode = user?.FindFirst(JwtRegisteredClaimNames.UniqueName)?.Value;
        Role = Enum.TryParse<Role>(user?.FindFirst(CustomClaimTypes.Role)?.Value, out var role) ? role : null;
    }

    /// <inheritdoc/>
    public Guid? StaffId { get; private init; }

    /// <inheritdoc/>
    public string? StaffCode { get; private init; }

    /// <inheritdoc/>
    public Role? Role { get; private init; }
}
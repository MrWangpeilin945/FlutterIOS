namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Auth;

/// <summary>
/// JWTに入れるClaimの名称を定義します
/// </summary>
public static class CustomClaimTypes
{
    /// <summary>
    /// JWTに持たせる権限
    /// </summary>
    public const string Role = "role";
}
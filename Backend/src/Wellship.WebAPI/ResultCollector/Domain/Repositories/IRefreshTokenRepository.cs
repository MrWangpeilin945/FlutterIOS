using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

/// <summary>
/// リフレッシュトークンリポジトリ
/// </summary>
public interface IRefreshTokenRepository
{
    /// <summary>
    /// 指定した職員IDのリフレッシュトークンを取得します
    /// </summary>
    /// <param name="staffId">職員ID</param>
    public ValueTask<RefreshToken?> GetRefreshTokenOrNullAsync(Guid staffId);
    /// <summary>
    /// 指定した職員IDのリフレッシュトークンを追加・更新します
    /// </summary>
    /// <param name="staffId">職員ID</param>
    /// <param name="refreshToken">リフレッシュトークン</param>
    public ValueTask UpdateRefreshTokenAsync(Guid staffId, RefreshToken refreshToken);
    /// <summary>
    /// 指定した職員IDのリフレッシュトークンを無効化（削除）します
    /// </summary>
    /// <param name="staffId">職員ID</param>
    public ValueTask ExpireRefreshTokenAsync(Guid staffId);
}

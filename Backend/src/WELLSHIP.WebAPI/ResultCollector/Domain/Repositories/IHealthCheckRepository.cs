namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

/// <summary>
/// ヘルスチェック用のリポジトリ
/// </summary>
public interface IHealthCheckRepository
{
    /// <summary>
    /// SELECT 1を投げてDB接続を確認します。
    /// </summary>
    /// <returns>接続が正常か</returns>
    public bool CheckDatabaseConnection();
}

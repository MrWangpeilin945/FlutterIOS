using System.Data.Common;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure;

/// <summary>
/// データベースのデータソースを管理するクラスのインターフェースです。
/// </summary>
public interface IDbDataSourceRegistry
{
    /// <summary>
    /// 指定したテナントキーに対応するデータソースを取得・生成します。
    /// </summary>
    /// <param name="key">テナントキー情報</param>
    /// <returns>データソース</returns>
    public ValueTask<DbDataSource> GetOrCreateAsync(string key);
}
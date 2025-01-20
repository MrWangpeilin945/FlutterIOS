using System.Data;
using System.Data.Common;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure;

/// <summary>
/// DBConnectionを生成・管理するクラスのインターフェースです。
/// </summary>
public interface IDbConnectionProvider
{
    /// <summary>
    /// 現在使用しているコネクションを返します。まだオープンされていない・破棄されている場合はコネクションを開きます。
    /// </summary>
    /// <returns>コネクション</returns>
    public ValueTask<DbConnection> GetOrOpenAsync();
    /// <summary>
    /// 現在使用しているコネクションを返します。オープンされていなければnullを返します。
    /// </summary>
    public DbConnection? GetCurrent();
}

/// <summary>
/// DBConnectionを生成・管理するクラスです。
/// </summary>
/// <param name="registory">データソースのレジストリ</param>
/// <param name="tenantProvider">接続先環境の情報を提供するプロバイダです</param>
public class DbConnectionProvider(IDbDataSourceRegistry registory, ITenantProvider tenantProvider) : IDbConnectionProvider, IDisposable
{
    private readonly IDbDataSourceRegistry _registory = registory;
    // NOTE: コネクションは1HTTPアクセスに対して最大1つを想定しています。平行で投げたい場合は別途実装が必要です。
    private DbConnection? _connection;
    // NOTE: サンプルのためHTTPヘッダからキーを取得しています。
    private readonly string _key = tenantProvider.TenantKey;

    /// <summary>
    /// 現在使用しているコネクションを返します。まだオープンされていない・破棄されている場合はコネクションを開きます。
    /// </summary>
    /// <returns>コネクション</returns>
    public async ValueTask<DbConnection> GetOrOpenAsync()
    {
        // NOTE: コネクションが開かれていない場合・開いた後閉じられた場合を想定しています。
        if (_connection is null || _connection.State == ConnectionState.Closed)
        {
            var dbDataSource = await _registory.GetOrCreateAsync(_key);
            _connection = await dbDataSource.OpenConnectionAsync();
        }
        return _connection;
    }

    /// <summary>
    /// HTTPレスポンスを返したあとこのプロバイダのインスタンスがライフサイクルに従ってDisposeされるとき、
    /// ConnectionもDisposeすることを意図しています。
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private bool _disposed;
    private void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }
        if (disposing)
        {
            // TODO: Disposeの実装これでいい？
            _connection?.Dispose();
            _disposed = true;
        }
    }

    /// <inheritdoc/>
    public DbConnection? GetCurrent() => _connection;
}
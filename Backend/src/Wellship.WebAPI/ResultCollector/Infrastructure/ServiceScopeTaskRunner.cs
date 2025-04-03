using Ryobi.Wellship.Core.Exceptions;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure;

/// <summary>
/// メインスレッドとは別のDIのスコープでFire-And-Forgetする仕組みを提供するためのインターフェースです
/// </summary>
public interface IServiceScopeTaskRunner
{
    /// <summary>
    /// 渡されたFuncを別のDIのスコープでFire-And-Forgetします
    /// </summary>
    /// <param name="func">別スレッド実行するFunc</param>
    /// <param name="token">キャンセルトークン</param>
    public Task Run(Func<Task> func, CancellationToken token = default);
    /// <summary>
    /// 渡されたFuncを別のDIのスコープでFire-And-Forgetします
    /// </summary>
    /// <typeparam name="T">別スレッド実行するFuncにDIする型</typeparam>
    /// <param name="func">別スレッド実行するFunc</param>
    /// <param name="token">キャンセルトークン</param>
    public Task Run<T>(Func<T, Task> func, CancellationToken token = default) where T : notnull;
    /// <summary>
    /// 渡されたFuncを別のDIのスコープでFire-And-Forgetします
    /// </summary>
    /// <typeparam name="T">別スレッド実行するFuncにDIする型</typeparam>
    /// <typeparam name="T2">別スレッド実行するFuncにDIする型</typeparam>
    /// <param name="func">別スレッド実行するFunc</param>
    /// <param name="token">キャンセルトークン</param>
    public Task Run<T, T2>(Func<T, T2, Task> func, CancellationToken token = default) where T : notnull
                                                                                      where T2 : notnull;
    /// <summary>
    /// 渡されたFuncを別のDIのスコープでFire-And-Forgetします
    /// </summary>
    /// <typeparam name="T">別スレッド実行するFuncにDIする型</typeparam>
    /// <typeparam name="T2">別スレッド実行するFuncにDIする型</typeparam>
    /// <typeparam name="T3">別スレッド実行するFuncにDIする型</typeparam>
    /// <param name="func">別スレッド実行するFunc</param>
    /// <param name="token">キャンセルトークン</param>
    public Task Run<T, T2, T3>(Func<T, T2, T3, Task> func, CancellationToken token = default) where T : notnull
                                                                                              where T2 : notnull
                                                                                              where T3 : notnull;
    /// <summary>
    /// 渡されたFuncを別のDIのスコープでFire-And-Forgetします
    /// </summary>
    /// <typeparam name="T">別スレッド実行するFuncにDIする型</typeparam>
    /// <typeparam name="T2">別スレッド実行するFuncにDIする型</typeparam>
    /// <typeparam name="T3">別スレッド実行するFuncにDIする型</typeparam>
    /// <typeparam name="T4">別スレッド実行するFuncにDIする型</typeparam>
    /// <param name="func">別スレッド実行するFunc</param>
    /// <param name="token">キャンセルトークン</param>
    public Task Run<T, T2, T3, T4>(Func<T, T2, T3, T4, Task> func, CancellationToken token = default) where T : notnull
                                                                                                      where T2 : notnull
                                                                                                      where T3 : notnull
                                                                                                      where T4 : notnull;
}

/// <summary>
/// メインスレッドとは別のDIのスコープでFire-And-Forgetする仕組みを提供するクラスです
/// </summary>
/// <param name="serviceScopeFactory">サービススコープを提供するFactory</param>
/// <param name="tenantProvider">HTTPContextからテナント情報を取得するプロバイダ</param>
public class ServiceScopeTaskRunner(IServiceScopeFactory serviceScopeFactory, ITenantProvider tenantProvider) : IServiceScopeTaskRunner
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
    // NOTE: ScopedでDIされるインスタンスを触らないよう、引き継ぐ情報=TenantKeyを取得して渡すようにしています。
    private readonly string _tenant = tenantProvider.TenantKey;
    // NOTE: 認証情報を引き継ぎません。用途上引き継ぐ必要があればInitialzeで引継ぎを実装をします。

    /// <inheritdoc/>
    public Task Run(Func<Task> func, CancellationToken token = default) =>
        Run(sp => func(), token);
    /// <inheritdoc/>
    public Task Run<T>(Func<T, Task> func, CancellationToken token = default) where T : notnull =>
        Run(sp => func(sp.GetRequiredService<T>()), token);
    /// <inheritdoc/>
    public Task Run<T, T2>(Func<T, T2, Task> func, CancellationToken token = default) where T : notnull where T2 : notnull =>
        Run(sp => func(sp.GetRequiredService<T>(), sp.GetRequiredService<T2>()), token);
    /// <inheritdoc/>
    public Task Run<T, T2, T3>(Func<T, T2, T3, Task> func, CancellationToken token = default) where T : notnull where T2 : notnull where T3 : notnull =>
        Run(sp => func(sp.GetRequiredService<T>(), sp.GetRequiredService<T2>(), sp.GetRequiredService<T3>()), token);
    /// <inheritdoc/>
    public Task Run<T, T2, T3, T4>(Func<T, T2, T3, T4, Task> func, CancellationToken token = default) where T : notnull where T2 : notnull where T3 : notnull where T4 : notnull =>
        Run(sp => func(sp.GetRequiredService<T>(), sp.GetRequiredService<T2>(), sp.GetRequiredService<T3>(), sp.GetRequiredService<T4>()), token);
    private Task Run(Func<IServiceProvider, Task> func, CancellationToken token)
    {
        // NOTE: コンテキストを完全に引き継がないため、トレース情報やその他引き継ぎたいものがあれば実装が必要です。
        using (ExecutionContext.SuppressFlow())
            return Task.Run(async () =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<ServiceScopeTaskRunner>>();
                try
                {
                    logger.Log(LogLevel.Error, "start");
                    var tenantProvider = scope.ServiceProvider.GetRequiredService<ITenantProvider>();
                    tenantProvider.OverrideTenantKeyOnlyIfNotSet(_tenant);
                    await func(scope.ServiceProvider);
                }
                catch (WellshipException wex)
                {
                    logger.Log(LogLevel.Warning, wex, wex.Message);
                }
                catch (Exception ex)
                {
                    logger.Log(LogLevel.Error, ex, ex.Message);
                }
            }, token);
    }
}

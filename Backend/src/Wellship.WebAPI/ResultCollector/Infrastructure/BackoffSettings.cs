
namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure;

/// <summary>
/// リトライ時の待機時間設定
/// </summary>
public record class BackoffSettings()
{
    /// <summary>
    /// リトライ回数
    /// </summary>
    public required int RetryCount { get; init; }
    /// <summary>
    /// 指数バックオフの場合の基底値
    /// </summary>
    public required int ExponentialBase { get; init; }

    /// <summary>
    /// リトライ時の待機回数
    /// </summary>
    public Func<int, TimeSpan> SleepDurationProvider => (retryAttempt) => TimeSpan.FromSeconds(Math.Pow(ExponentialBase, retryAttempt));
}


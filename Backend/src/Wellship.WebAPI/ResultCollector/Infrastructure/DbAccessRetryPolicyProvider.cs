
using Npgsql;

using Polly;
using Polly.Retry;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure;

/// <summary>
/// データベースアクセスのリトライポリシーを提供するプロバイダです
/// </summary>
public interface IDbAccessRetryPolicyProvider
{
    /// <summary>
    /// データベースアクセスのリトライポリシー
    /// </summary>
    public ResiliencePipeline ResiliencePipeline { get; }
}

/// <inheritdoc/>
public class DbAccessRetryPolicyProvider : IDbAccessRetryPolicyProvider
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="backoffSettings">リトライの設定</param>
    public DbAccessRetryPolicyProvider(BackoffSettings backoffSettings)
    {
        var builder = new ResiliencePipelineBuilder();
        builder.AddRetry(new RetryStrategyOptions()
        {
            // IsTransient: https://github.com/npgsql/npgsql/blob/cf9d2433bc653f363705296b5804a9658bb49083/src/Npgsql/PostgresException.cs#L205
            // ErrorCodes : https://www.npgsql.org/doc/api/Npgsql.PostgresErrorCodes.html
            ShouldHandle = new PredicateBuilder().Handle<NpgsqlException>(nex => nex.IsTransient)
                                                 .Handle<IOException>()
                                                 .Handle<TimeoutException>(),
            MaxRetryAttempts = backoffSettings.RetryCount,
            Delay = TimeSpan.FromSeconds(backoffSettings.ExponentialBase),
            BackoffType = DelayBackoffType.Exponential,
        });
        ResiliencePipeline = builder.Build();
    }

    /// <inheritdoc/>
    public ResiliencePipeline ResiliencePipeline { get; }
}
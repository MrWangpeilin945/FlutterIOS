using System.Data;
using System.Data.Common;

using Polly;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Resilience;


/// <summary>
/// リトライを提供するDbTransactionのラッパー
/// </summary>
/// <param name="innerDbTransaction">実体のDbTransaction</param>
/// <param name="resiliencePipeline">リトライポリシー</param>
public class ResilienceDbTransaction(DbTransaction innerDbTransaction, ResiliencePipeline resiliencePipeline) : DbTransaction
{
    /// <summary>
    /// DbTransactionの実体
    /// </summary>
    public DbTransaction InnerDbTransaction { get; } = innerDbTransaction;
    /// <inheritdoc/>
    public override IsolationLevel IsolationLevel => InnerDbTransaction.IsolationLevel;
    /// <inheritdoc/>
    protected override DbConnection? DbConnection
    => InnerDbTransaction.Connection switch
    {
        ResilienceDbConnection => InnerDbTransaction.Connection,
        System.Data.Common.DbConnection => new ResilienceDbConnection(InnerDbTransaction.Connection, resiliencePipeline),
        _ => null
    };
    /// <inheritdoc/>
    public override void Commit() => resiliencePipeline.Execute(InnerDbTransaction.Commit);
    /// <inheritdoc/>
    public override Task CommitAsync(CancellationToken cancellationToken = default)
    => resiliencePipeline.ExecuteAsync(async (token) => await InnerDbTransaction.CommitAsync(cancellationToken), cancellationToken).AsTask();
    /// <inheritdoc/>
    public override void Rollback() => resiliencePipeline.Execute(InnerDbTransaction.Rollback);
    /// <inheritdoc/>
    public override Task RollbackAsync(CancellationToken cancellationToken = default)
    => resiliencePipeline.ExecuteAsync(async (token) => await InnerDbTransaction.RollbackAsync(cancellationToken), cancellationToken).AsTask();
    /// <inheritdoc/>
    public override void Save(string name) => resiliencePipeline.Execute(() => InnerDbTransaction.Save(name));
    /// <inheritdoc/>
    public override Task SaveAsync(string name, CancellationToken cancellationToken = default)
    => resiliencePipeline.ExecuteAsync(async (token) => await InnerDbTransaction.SaveAsync(name, cancellationToken), cancellationToken).AsTask();
    /// <inheritdoc/>
    public override void Rollback(string name) => resiliencePipeline.Execute(() => InnerDbTransaction.Rollback(name));
    /// <inheritdoc/>
    public override Task RollbackAsync(string name, CancellationToken cancellationToken = default)
    => resiliencePipeline.ExecuteAsync(async (token) => await InnerDbTransaction.RollbackAsync(name, cancellationToken), cancellationToken).AsTask();
    /// <inheritdoc/>
    public override void Release(string name) => resiliencePipeline.Execute(() => InnerDbTransaction.Release(name));
    /// <inheritdoc/>
    public override Task ReleaseAsync(string name, CancellationToken cancellationToken = default)
    => resiliencePipeline.ExecuteAsync(async (token) => await InnerDbTransaction.ReleaseAsync(name, cancellationToken), cancellationToken).AsTask();
    /// <inheritdoc/>
    public override bool SupportsSavepoints => InnerDbTransaction.SupportsSavepoints;
    /// <inheritdoc/>
    protected override void Dispose(bool disposing) => InnerDbTransaction.Dispose();
    /// <inheritdoc/>
    public override ValueTask DisposeAsync() => InnerDbTransaction.DisposeAsync();
}
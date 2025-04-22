
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

using Polly;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Resilience;

/// <summary>
/// DBCommandにリトライ機能を持たせるラッパークラスです
/// </summary>
/// <param name="dbCommand">実体のDbCommand</param>
/// <param name="resiliencePipeline">リトライパイプライン</param>
public class ResilienceDbCommand(DbCommand dbCommand, ResiliencePipeline resiliencePipeline) : DbCommand
{
    /// <summary>
    /// 実体のDbCommand
    /// </summary>
    public DbCommand InternalDbCommand { get; } = dbCommand;
    /// <inheritdoc/>
    public override int ExecuteNonQuery() => resiliencePipeline.Execute(InternalDbCommand.ExecuteNonQuery);
    /// <inheritdoc/>
    public override Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
        => resiliencePipeline.ExecuteAsync(async token => await InternalDbCommand.ExecuteNonQueryAsync(cancellationToken), cancellationToken).AsTask();
    /// <inheritdoc/>
    public override object? ExecuteScalar() => resiliencePipeline.Execute(InternalDbCommand.ExecuteScalar);
    /// <inheritdoc/>
    public override Task<object?> ExecuteScalarAsync(CancellationToken cancellationToken)
        => resiliencePipeline.ExecuteAsync(async token => await InternalDbCommand.ExecuteScalarAsync(cancellationToken), cancellationToken).AsTask();
    /// <inheritdoc/>
    protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) => InternalDbCommand.ExecuteReader(behavior);
    /// <inheritdoc/>
    protected override Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken)
        => resiliencePipeline.ExecuteAsync(async token => await InternalDbCommand.ExecuteReaderAsync(behavior, cancellationToken), cancellationToken).AsTask();
    /// <inheritdoc/>
    [AllowNull]
    public override string CommandText { get => InternalDbCommand.CommandText; set => InternalDbCommand.CommandText = value; }
    /// <inheritdoc/>
    public override int CommandTimeout { get => InternalDbCommand.CommandTimeout; set => InternalDbCommand.CommandTimeout = value; }
    /// <inheritdoc/>
    public override CommandType CommandType { get => InternalDbCommand.CommandType; set => InternalDbCommand.CommandType = value; }
    /// <inheritdoc/>
    public override bool DesignTimeVisible { get => InternalDbCommand.DesignTimeVisible; set => InternalDbCommand.DesignTimeVisible = true; }
    /// <inheritdoc/>
    public override UpdateRowSource UpdatedRowSource { get => InternalDbCommand.UpdatedRowSource; set => InternalDbCommand.UpdatedRowSource = value; }
    /// <inheritdoc/>
    protected override DbConnection? DbConnection { get => InternalDbCommand.Connection; set => InternalDbCommand.Connection = value; }
    /// <inheritdoc/>
    protected override DbParameterCollection DbParameterCollection => InternalDbCommand.Parameters;
    /// <inheritdoc/>
    protected override DbTransaction? DbTransaction { get => InternalDbCommand.Transaction; set => InternalDbCommand.Transaction = value; }
    /// <inheritdoc/>
    public override void Cancel() => InternalDbCommand.Cancel();
    /// <inheritdoc/>
    public override void Prepare() => InternalDbCommand.Prepare();
    /// <inheritdoc/>
    public override Task PrepareAsync(CancellationToken cancellationToken = default) => InternalDbCommand.PrepareAsync(cancellationToken);
    /// <inheritdoc/>
    protected override DbParameter CreateDbParameter() => InternalDbCommand.CreateParameter();
    /// <inheritdoc/>
    public override ValueTask DisposeAsync() => InternalDbCommand.DisposeAsync();
}

using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

using Polly;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Resilience;

/// <summary>
/// リトライ機能を持たせたDBCommandを発行するDbConnectionのラッパークラスです。
/// </summary>
/// <param name="dbConnection"></param>
/// <param name="resiliencePipeline"></param>
public class ResilienceDbConnection(DbConnection dbConnection, ResiliencePipeline resiliencePipeline) : DbConnection
{
    /// <summary>
    /// 実体のDbConnection
    /// </summary>
    public DbConnection InnerDbConnection { get; } = dbConnection;
    /// <inheritdoc/>
    public override void Open() => resiliencePipeline.Execute(InnerDbConnection.Open);
    /// <inheritdoc/>
    public override Task OpenAsync(CancellationToken cancellationToken)
        => resiliencePipeline.ExecuteAsync(async token => await InnerDbConnection.OpenAsync(cancellationToken), cancellationToken).AsTask();
    /// <inheritdoc/>
    [AllowNull]
    public override string ConnectionString { get => InnerDbConnection.ConnectionString; set => InnerDbConnection.ConnectionString = value; }
    /// <inheritdoc/>
    public override int ConnectionTimeout => InnerDbConnection.ConnectionTimeout;
    /// <inheritdoc/>
    public override string Database => InnerDbConnection.Database;
    /// <inheritdoc/>
    public override string DataSource => InnerDbConnection.DataSource;
    /// <inheritdoc/>
    public override ConnectionState State => InnerDbConnection.State;
    /// <inheritdoc/>
    protected override DbCommand CreateDbCommand() => new ResilienceDbCommand(InnerDbConnection.CreateCommand(), resiliencePipeline);
    /// <inheritdoc/>
    public override bool CanCreateBatch => InnerDbConnection.CanCreateBatch;
    /// <inheritdoc/>
    protected override DbBatch CreateDbBatch() => InnerDbConnection.CreateBatch();
    /// <inheritdoc/>
    protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
    => resiliencePipeline.Execute(() => new ResilienceDbTransaction(InnerDbConnection.BeginTransaction(isolationLevel), resiliencePipeline));
    /// <inheritdoc/>
    protected override ValueTask<DbTransaction> BeginDbTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken)
     => resiliencePipeline.ExecuteAsync(async token =>
     {
         var transaction = await InnerDbConnection.BeginTransactionAsync(isolationLevel, cancellationToken);
         return (DbTransaction)new ResilienceDbTransaction(transaction, resiliencePipeline);
     }, cancellationToken);
    /// <inheritdoc/>
    public override void EnlistTransaction(System.Transactions.Transaction? transaction) => InnerDbConnection.EnlistTransaction(transaction);
    /// <inheritdoc/>
    public override void Close() => InnerDbConnection.Close();
    /// <inheritdoc/>
    public override Task CloseAsync() => InnerDbConnection.CloseAsync();
    /// <inheritdoc/>
    protected override void Dispose(bool disposing) => InnerDbConnection.Dispose();
    /// <inheritdoc/>
    public override ValueTask DisposeAsync() => InnerDbConnection.DisposeAsync();
    /// <inheritdoc/>
    public override string ServerVersion => InnerDbConnection.ServerVersion;
    /// <inheritdoc/>
    public override DataTable GetSchema() => InnerDbConnection.GetSchema();
    /// <inheritdoc/>
    public override DataTable GetSchema(string collectionName) => InnerDbConnection.GetSchema(collectionName);
    /// <inheritdoc/>
    public override DataTable GetSchema(string collectionName, string?[] restrictions) => InnerDbConnection.GetSchema(collectionName, restrictions);
    /// <inheritdoc/>
    public override Task<DataTable> GetSchemaAsync(CancellationToken cancellationToken = default)
        => InnerDbConnection.GetSchemaAsync(cancellationToken);
    /// <inheritdoc/>
    public override Task<DataTable> GetSchemaAsync(string collectionName, CancellationToken cancellationToken = default)
        => InnerDbConnection.GetSchemaAsync(collectionName, cancellationToken);
    /// <inheritdoc/>
    public override Task<DataTable> GetSchemaAsync(string collectionName, string?[] restrictions, CancellationToken cancellationToken = default)
        => InnerDbConnection.GetSchemaAsync(collectionName, restrictions, cancellationToken);
    /// <inheritdoc/>
    public override void ChangeDatabase(string dbName)
        => InnerDbConnection.ChangeDatabase(dbName);
}
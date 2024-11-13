namespace Ryobi.Wellship.WebAPI.DapperSample.Infrastructure.PostgreSQL.Entities;

/// <summary>
/// pg_tablesのモデル
/// </summary>
internal record class PgTable()
{
    public string SchemaName { get; set; } = null!;
    public string TableName { get; set; } = null!;
    public string TableOwner { get; set; } = null!;
    public string? TableSpace { get; set; }
    public bool HasIndexes { get; set; }
    public bool HasRules { get; set; }
    public bool HasTriggers { get; set; }
    public bool RowSecurity { get; set; }
}

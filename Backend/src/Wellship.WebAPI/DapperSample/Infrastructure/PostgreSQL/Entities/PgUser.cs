namespace Ryobi.Wellship.WebAPI.DapperSample.Infrastructure.PostgreSQL.Entities;

/// <summary>
/// pg_userのモデル
/// </summary>
internal record class PgUser()
{
    public string UseName { get; set; } = null!;
    public int UseSysId { get; set; }
    public bool UseCreateDb { get; set; }
    public bool UseSuper { get; set; }
    public bool UseRepl { get; set; }
    public bool UseByPassRls { get; set; }
}

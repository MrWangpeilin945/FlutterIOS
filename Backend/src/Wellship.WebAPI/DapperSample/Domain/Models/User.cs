namespace Ryobi.Wellship.WebAPI.DapperSample.Domain.Models;

/// <summary>
/// ユーザーのモデル
/// </summary>
public record class User()
{
    /// <summary>
    /// ユーザー名
    /// </summary>
    public required string Name { get; init; }
    /// <summary>
    /// SysId
    /// </summary>
    public required int SysId { get; init; }
    /// <summary>
    /// CreateDb
    /// </summary>
    public required bool UseCreateDb { get; init; }
    /// <summary>
    /// Super
    /// </summary>
    public required bool UseSuper { get; init; }
    /// <summary>
    /// Repl
    /// </summary>
    public required bool UseRepl { get; init; }
    /// <summary>
    /// ByPassRls
    /// </summary>
    public required bool UseByPassRls { get; init; }
    /// <summary>
    /// テーブル
    /// </summary>
    public required Table[] Tables { get; init; }
}

namespace Ryobi.Wellship.WebAPI.DapperSample.Domain.Models;

/// <summary>
/// テーブルのモデル
/// </summary>
public record class Table()
{
    /// <summary>
    /// テーブル名
    /// </summary>
    public required string Name { get; init; }
    /// <summary>
    /// スキーマ名
    /// </summary>
    public required string SchemaName { get; init; }
    /// <summary>
    /// インデックスの有無
    /// </summary>
    public required bool HasIndex { get; init; }
    /// <summary>
    /// ルールの有無
    /// </summary>
    public required bool HasRules { get; init; }
    /// <summary>
    /// トリガーの有無
    /// </summary>
    public required bool HasTriggers { get; init; }
    /// <summary>
    /// RowSecurityの有無
    /// </summary>
    public required bool RowSecurity { get; init; }
}

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 連携処理結果ログ明細
/// </summary>
public sealed class IntegrationResultLogDetail
{
    /// <summary>
    /// 表示順
    /// </summary>
    public required int OrderNumber { get; init; }

    /// <summary>
    /// 機能コード
    /// </summary>
    public required string FunctionCode { get; init; }

    /// <summary>
    /// 機能名
    /// </summary>
    public required string FunctionName { get; init; }

    /// <summary>
    /// 発生源
    /// </summary>
    public required string EventSource { get; init; }

    /// <summary>
    /// 詳細結果コード
    /// </summary>
    public required string ResultDetailCode { get; init; }

    /// <summary>
    /// 詳細結果メッセージ
    /// </summary>
    public required string ResultDetailMessage { get; init; }

    /// <summary>
    /// プロパティ
    /// 具体的な項目名と値です。
    /// </summary>
    public required IEnumerable<IntegrationResultLogDetailProperty> Properties { get; init; }
}

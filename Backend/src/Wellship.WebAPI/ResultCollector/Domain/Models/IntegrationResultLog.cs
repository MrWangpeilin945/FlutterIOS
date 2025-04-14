namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 連携処理結果ログ
/// </summary>
public sealed class IntegrationResultLog
{
    /// <summary>
    /// ログID
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// 概要
    /// </summary>
    public required string Summary { get; init; }

    /// <summary>
    /// ログレベル
    /// </summary>
    public required LogLevel LogLevel { get; init; }

    /// <summary>
    /// 処理結果コード
    /// </summary>
    public required string ResultCode { get; init; }

    /// <summary>
    /// 処理結果コード名称
    /// </summary>
    public required string ResultCodeName { get; init; }

    /// <summary>
    /// 機能コード
    /// </summary>
    public required string FunctionCode { get; init; }

    /// <summary>
    /// 機能名
    /// </summary>
    public required string FuctionName { get; init; }

    /// <summary>
    /// 発生日時
    /// </summary>
    public required DateTimeOffset OccurredAt { get; init; }

    /// <summary>
    /// ログ明細
    /// </summary>
    public required List<IntegrationResultLogDetail> Details { get; init; }
}

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 連携処理結果ログ（書き込み用モデル）
/// </summary>
public sealed class IntegrationResultLogWriteModel
{
    /// <summary>
    /// ログID
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// 処理結果コード
    /// </summary>
    public required string ResultCode { get; init; }

    /// <summary>
    /// ログレベル
    /// </summary>
    public required LogLevel LogLevel { get; init; }

    /// <summary>
    /// 機能コード
    /// </summary>
    public required string FunctionCode { get; init; }

    /// <summary>
    /// 概要
    /// </summary>
    public required string Summary { get; init; }

    /// <summary>
    /// ログ明細
    /// </summary>
    public required List<IntegrationResultLogDetailWriteModel> Details { get; init; }
}

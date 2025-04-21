namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 連携処理結果ログ明細プロパティ（書き込み用モデル）
/// </summary>
public sealed class IntegrationResultLogDetailPropertyWriteModel
{
    /// <summary>
    /// 項目名
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// 値
    /// </summary>
    public required string Value { get; init; }
}

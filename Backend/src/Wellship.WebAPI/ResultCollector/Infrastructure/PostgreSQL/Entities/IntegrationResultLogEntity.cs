using System.Text.Json;
using System.Text.Json.Serialization;

using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;

/// <summary>
/// 連携処理結果ログエンティティ
/// </summary>
public class IntegrationResultLogEntity
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
    };

    /// <summary>
    /// 連携処理結果ログID
    /// </summary>
    public required Guid LogId { get; set; }

    /// <summary>
    /// 連携処理結果コード
    /// </summary>
    public required string LogResultCode { get; set; }

    /// <summary>
    /// 連携処理結果名
    /// </summary>
    public required string LogResultName { get; set; }

    /// <summary>
    /// 機能ID
    /// </summary>
    public required string LogFunctionCode { get; set; }

    /// <summary>
    /// 機能名
    /// </summary>
    public required string LogFunctionName { get; set; }

    /// <summary>
    /// ログレベル
    /// </summary>
    public required int LogLevel { get; set; }

    /// <summary>
    /// 概要
    /// </summary>
    public required string LogSummary { get; set; }

    /// <summary>
    /// ログの作成日時
    /// </summary>
    public required DateTimeOffset LogCreatedAt { get; set; }

    /// <summary>
    /// 明細の表示順
    /// </summary>
    public required int DetailOrderNumber { get; set; }

    /// <summary>
    /// 明細の機能コード
    /// </summary>
    public required string DetailFunctionCode { get; set; }

    /// <summary>
    /// 明細の機能名
    /// </summary>
    public required string DetailFunctionName { get; set; }

    /// <summary>
    /// 明細の発生源
    /// </summary>
    public required string DetailEventSource { get; set; }

    /// <summary>
    /// 詳細結果コード
    /// </summary>
    public required string ResultDetailCode { get; set; }

    /// <summary>
    /// 詳細結果メッセージ
    /// </summary>
    public required string ResultDetailMessage { get; set; }

    /// <summary>
    /// プロパティ（JSON文字列）
    /// 具体的な項目名と値です。
    /// </summary>
    public required string DetailPropertiesText { get; set; }

    /// <summary>
    /// プロパティ
    /// JSON文字列をデシリアライズする
    /// </summary>
    public IEnumerable<IntegrationResultLogDetailProperty> DetailProperties
    {
        get
        {
            return JsonSerializer.Deserialize<IEnumerable<IntegrationResultLogDetailProperty>>(DetailPropertiesText, JsonSerializerOptions) ?? [];
        }
    }
}

using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 連携処理結果ログ明細（書き込み用モデル）
/// </summary>
public sealed class IntegrationResultLogDetailWriteModel
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    /// <summary>
    /// 機能コード
    /// </summary>
    public required string FunctionCode { get; init; }

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
    public required Dictionary<string, string> Properties { get; init; }

    /// <summary>
    /// JSONシリアライズされたプロパティ
    /// </summary>
    public string PropertiesJsonString
    {
        get
        {
            if (Properties is null)
            {
                // 付加情報がnullなら、jsonの空オブジェクトとする
                return "{}";
            }

            return JsonSerializer.Serialize(Properties, JsonSerializerOptions);
        }
    }
}

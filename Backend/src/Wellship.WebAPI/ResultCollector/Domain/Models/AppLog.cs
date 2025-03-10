using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// アプリケーションログ
/// </summary>
public class AppLog
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    /// <summary>
    /// ログレベル
    /// </summary>
    public required LogLevel LogLevel { get; init; }

    /// <summary>
    /// メッセージ
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    /// 付加情報
    /// </summary>
    public Dictionary<string, object>? Details { get; init; }

    /// <summary>
    /// 付加情報のJSON文字列
    /// </summary>
    public string DetailsJsonString
    {
        get
        {
            if (Details is null)
            {
                // 付加情報がnullなら、jsonの空オブジェクトとする
                return "{}";
            }

            return JsonSerializer.Serialize(Details, JsonSerializerOptions);
        }
    }
}

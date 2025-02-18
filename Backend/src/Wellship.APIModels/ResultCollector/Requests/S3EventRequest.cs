using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Requests;

/// <summary>
/// S3イベントのリクエストモデル
/// </summary>
public class S3EventRequest
{
    /// <summary>
    /// バケット名
    /// </summary>
    [JsonPropertyName("bucketName")]
    public string BucketName { get; set; } = null!;

    /// <summary>
    /// オブジェクトキー
    /// </summary>
    [JsonPropertyName("objectKey")]
    public string ObjectKey { get; set; } = null!;
}

using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Requests;

/// <summary>
/// 実施有無と中止理由を束ねたリクエストモデル
/// </summary>
public class ExecutionsRequest
{
    /// <summary>
    /// 検査項目ごとの実施有無と中止理由
    /// </summary>
    [JsonPropertyName("executions")]
    public ExecutionRequest[] Executions { get; set; } = [];
}

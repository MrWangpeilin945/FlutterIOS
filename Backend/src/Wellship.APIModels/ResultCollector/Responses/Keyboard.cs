using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// キーボード
/// </summary>
public class Keyboard
{
    /// <summary>
    /// キーボード種別
    /// </summary>
    [JsonPropertyName("keyboardType")]
    public required int KeyboardType { get; init; }

    /// <summary>
    /// キーボード入力値
    /// </summary>
    [JsonPropertyName("values")]
    public required string[] Values { get; init; }
}

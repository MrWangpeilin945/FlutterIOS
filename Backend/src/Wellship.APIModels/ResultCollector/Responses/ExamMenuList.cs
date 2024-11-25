using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 検査メニューリスト
/// </summary>
public class ExamMenuList
{
    /// <summary>
    /// 検査メニューリスト
    /// </summary>
    [JsonPropertyName("examMenus")]
    public required ExamMenu[] ExamMenus { get; init; }
}

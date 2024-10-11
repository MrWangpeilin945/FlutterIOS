using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 検査メニュー
/// </summary>
public class ExamMenu
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ExamMenu(int examMenuId, string examMenuName)
    {
        ExamMenuId = examMenuId;
        ExamMenuName = examMenuName;
    }

    /// <summary>
    /// 検査メニューID
    /// </summary>
    [JsonPropertyName("examMenuId")]
    public int ExamMenuId { get; }

    /// <summary>
    /// 検査メニュー名
    /// </summary>
    [JsonPropertyName("examMenuName")]
    public string ExamMenuName { get; }
}

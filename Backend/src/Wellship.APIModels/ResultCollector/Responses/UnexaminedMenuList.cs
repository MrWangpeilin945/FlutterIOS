using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 未受診の検査メニューリスト
/// </summary>
public class UnexaminedMenuList
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public UnexaminedMenuList(int consultId, int examineeId, string examineeName, ICollection<ExamMenu> examMenus)
    {
        ConsultId = consultId;
        ExamineeId = examineeId;
        ExamineeName = examineeName;
        UnexaminedMenus = examMenus.ToArray();
    }

    /// <summary>
    /// 受診ID
    /// </summary>
    [JsonPropertyName("consultId")]
    public int ConsultId { get; }

    /// <summary>
    /// 受診者ID
    /// </summary>
    [JsonPropertyName("examineeId")]
    public int ExamineeId { get; }

    /// <summary>
    /// 受診者名
    /// </summary>
    [JsonPropertyName("examineeName")]
    public string ExamineeName { get; }

    /// <summary>
    /// 未受診の検査メニューリスト
    /// </summary>
    [JsonPropertyName("unexaminedMenus")]
    public ExamMenu[] UnexaminedMenus { get; }
}

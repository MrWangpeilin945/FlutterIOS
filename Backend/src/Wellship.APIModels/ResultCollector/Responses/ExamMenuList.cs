using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 検査メニューリスト
/// </summary>
public class ExamMenuList
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ExamMenuList(ExamMenu[] examMenus)
    {
        ExamMenus = examMenus;
    }

    /// <summary>
    /// 検査メニューリスト
    /// </summary>
    [JsonPropertyName("examMenus")]
    public ExamMenu[] ExamMenus { get; }
}

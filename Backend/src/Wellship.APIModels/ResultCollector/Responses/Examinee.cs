using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Responses;

/// <summary>
/// 受診者情報
/// </summary>
public class Examinee
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Examinee(int examineeId, string name, string kanaName, DateOnly birthdate, int sex, 
                    string[] organizations, bool sameNameAlert, int examDateAge)
    {
        ExamineeId = examineeId;
        Name = name;
        Name = kanaName;
        Birthdate = birthdate;
        Sex = sex;
        Organizations = organizations;
        SameNameAlert = sameNameAlert;
        ExamDateAge = examDateAge;
    }

    /// <summary>
    /// 受診者ID
    /// </summary>
    [JsonPropertyName("examineeId")]
    public int ExamineeId { get; }

    /// <summary>
    /// 氏名
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; } = "";

    /// <summary>
    /// カナ氏名
    /// </summary>
    [JsonPropertyName("kanaName")]
    public string KanaName { get; } = "";

    /// <summary>
    /// 生年月日
    /// </summary>
    [JsonPropertyName("birthdate")]
    public DateOnly Birthdate { get; }    

    /// <summary>
    /// 性別
    /// </summary>
    [JsonPropertyName("sex")]
    public int Sex { get; }    

    /// <summary>
    /// 事業所名
    /// </summary>
    [JsonPropertyName("organizations")]
    public string[] Organizations { get; }    

    /// <summary>
    /// 同姓同名アラート
    /// </summary>
    [JsonPropertyName("sameNameAlert")]
    public bool SameNameAlert { get; }    

    /// <summary>
    /// 受診日年齢
    /// </summary>
    [JsonPropertyName("examDateAge")]
    public int ExamDateAge { get; }    

}

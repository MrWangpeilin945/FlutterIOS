namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 検査結果のエンティティ
/// </summary>
public class ExamResultEntity
{
    /// <summary>
    /// 受診ID
    /// </summary>
    public int ConsultId { get; set; }

    /// <summary>
    /// 検査項目ID
    /// </summary>
    public int ExamItemId { get; set; }

    /// <summary>
    /// 検査項目明細ID
    /// </summary>
    public int ExamItemDetailId { get; set; }

    /// <summary>
    /// 値
    /// </summary>
    public required string Value { get; set; }
    
}

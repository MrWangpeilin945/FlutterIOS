namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 過去検査結果のエンティティ
/// </summary>
public class PreviousResultEntity
{
    /// <summary>
    /// 受診ID
    /// </summary>
    public required int ConsultId { get; set; }

    /// <summary>
    /// 健診日
    /// </summary>
    public required DateTime ExamDate { get; set; }

    /// <summary>
    /// 検査項目ID
    /// </summary>
    public required int ExamItemId { get; set; }

    /// <summary>
    /// 検査項目明細ID
    /// </summary>
    public required int ExamItemDetailId { get; set; }

    /// <summary>
    /// 値
    /// </summary>
    public required string Value { get; set; }
}

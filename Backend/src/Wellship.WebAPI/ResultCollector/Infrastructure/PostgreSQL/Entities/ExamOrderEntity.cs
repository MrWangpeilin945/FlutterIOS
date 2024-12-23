namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;

/// <summary>
/// 検査依頼のエンティティ
/// </summary>
public class ExamOrderEntity
{
    /// <summary>
    /// 受診ID
    /// </summary>
    public Guid ConsultId { get; set; }

    /// <summary>
    /// 検査項目ID
    /// </summary>
    public int ExamItemId { get; set; }

    /// <summary>
    /// 検査項目明細ID
    /// </summary>
    public int ExamItemDetailId { get; set; }

}

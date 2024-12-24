namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;

/// <summary>
/// 検査結果登録のエンティティ
/// </summary>
public class ExamResultRegisteEntity
{
    /// <summary>
    /// 検査項目明細ID
    /// </summary>
    public required int ExamItemDetailId { get; set; }

    /// <summary>
    /// 値
    /// </summary>
    public required string Value { get; set; }
}

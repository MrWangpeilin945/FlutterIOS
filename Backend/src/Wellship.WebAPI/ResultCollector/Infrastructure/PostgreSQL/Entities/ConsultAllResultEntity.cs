namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;

/// <summary>
/// 受診に紐づく全ての検査結果のエンティティ
/// </summary>
public class ConsultAllResultEntity
{
    /// <summary>
    ///検査メニューID
    /// </summary>
    public required int ExamMenuId { get; set; }

    /// <summary>
    /// 検査メニュー名
    /// </summary>
    public required string ExamMenuName { get; set; }

    /// <summary>
    /// 検査項目ID
    /// </summary>
    public required int ExamItemId { get; set; }

    /// <summary>
    /// 検査項目名
    /// </summary>
    public required string ExamItemName { get; set; }

    /// <summary>
    /// 検査項目明細ID
    /// </summary>
    public required int ExamItemDetailId { get; set; }

    /// <summary>
    /// 検査項目明細名
    /// </summary>
    public required string ExamItemDetailName { get; set; }

    /// <summary>
    /// メニュー表示順
    /// </summary>
    public required string MenuOrderNumber { get; set; }

    /// <summary>
    /// 項目表示順
    /// </summary>
    public required string ItemOrderNumber { get; set; }

    /// <summary>
    /// 項目明細表示順
    /// </summary>
    public required string ItemDetailOrderNumber { get; set; }

    /// <summary>
    /// 今回値
    /// </summary>
    public string? CurrentResult { get; set; }

    /// <summary>
    /// 過去値
    /// </summary>
    public string? PastResult { get; set; }

    /// <summary>
    /// 過去検査日
    /// </summary>
    public DateOnly? PastDate { get; set; }

    /// <summary>
    /// 直近判定
    /// </summary>
    public required bool IsRecent { get; set; }

    /// <summary>
    /// 検査進捗状況
    /// </summary>
    public required int Status { get; set; }
}
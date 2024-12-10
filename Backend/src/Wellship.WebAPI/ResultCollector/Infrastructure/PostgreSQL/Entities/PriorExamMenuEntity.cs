namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;

/// <summary>
/// 前提検査メニューの設定エンティティ
/// </summary>
public class PriorExamMenuEntity
{
    /// <summary>
    /// 現在の検査メニューID
    /// </summary>
    public int CurrentExamMenuId { get; set; }

    /// <summary>
    /// 前提検査メニューID
    /// </summary>
    public int PriorExamMenuId { get; set; }
}

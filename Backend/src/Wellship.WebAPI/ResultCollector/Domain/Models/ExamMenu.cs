namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 検査メニュー
/// </summary>
public class ExamMenu
{
    /// <summary>
    /// 検査メニューID
    /// </summary>
    public required int MenuId { get; init; }

    /// <summary>
    /// 検査メニュー名
    /// </summary>
    public required string MenuName { get; init; }
}

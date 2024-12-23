namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 検査メニュー特記マスタ_受診特記
/// </summary>
public class MenuNoteConsult
{
    /// <summary>
    /// 検査特記コード
    /// </summary>
    public required string Code { get; init; }
}

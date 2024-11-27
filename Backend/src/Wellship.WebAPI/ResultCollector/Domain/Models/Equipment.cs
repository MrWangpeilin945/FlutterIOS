namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 検査機器
/// </summary>
public class Equipment
{
    /// <summary>
    /// 検査機器ID
    /// </summary>
    public required int EquipmentId { get; init; }

    /// <summary>
    /// 検査機器名
    /// </summary>
    public required string EquipmentName { get; init; }

    /// <summary>
    /// アプリ起動URL
    /// </summary>
    public required string AppLaunchUrl { get; init; }

    /// <summary>
    /// 処理スクリプトURL
    /// </summary>
    public required string ProcessingScriptUrl { get; init; }
}
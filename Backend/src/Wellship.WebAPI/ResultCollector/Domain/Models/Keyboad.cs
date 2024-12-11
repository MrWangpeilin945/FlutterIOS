using Ryobi.Wellship.Core.Enums;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// キーボード入力値リスト
/// </summary>
public class Keyboard
{
    /// <summary>
    /// ID
    /// </summary>
    public required int OptionId { get; init; }

    /// <summary>
    /// 検査項目明細ID
    /// </summary>
    public required int ExamItemDetailId { get; init; }

    /// <summary>
    /// 入力値
    /// </summary>
    public required string Value { get; init; }

}
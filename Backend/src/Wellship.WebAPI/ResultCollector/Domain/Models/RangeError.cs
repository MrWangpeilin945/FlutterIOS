using Ryobi.Wellship.Core.Enums;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 検査正常値のエラー
/// </summary>
public class RangeError
{
    /// <summary>
    /// 範囲ID
    /// </summary>
    public Guid RangeId { get; init; }

    /// <summary>
    /// 範囲名称
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// 検査項目明細ID
    /// </summary>
    public required int ExamItemDetailId { get; init; }

    /// <summary>
    /// 値上限
    /// </summary>
    public required decimal MaxValue { get; init; }

    /// <summary>
    /// 値下限
    /// </summary>
    public required decimal MinValue { get; init; }

    /// <summary>
    /// エラーレベル
    /// </summary>
    public required InputErrorLevel ErrorLevel { get; init; }

    /// <summary>
    /// エラーレベルに応じた表示用の文言
    /// </summary>
    public string Message
    {
        get
        {
            return ErrorLevel switch
            {

                InputErrorLevel.正常 => "",
                InputErrorLevel.警告 => "入力値を確認してください。",
                InputErrorLevel.異常 => "入力に誤りがあります。",
                _ => throw new ArgumentOutOfRangeException(nameof(ErrorLevel), $"未対応のエラーレベル: {ErrorLevel}")
            };
        }
    }
}

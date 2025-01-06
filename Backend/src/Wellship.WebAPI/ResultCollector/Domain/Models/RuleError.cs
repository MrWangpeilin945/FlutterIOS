using Ryobi.Wellship.Core.Enums;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// ルールにより検証した結果を返すエラーオブジェクト
/// </summary>
public class RuleError
{
    /// <summary>
    /// エラーレベル
    /// </summary>
    public required InputErrorLevel ErrorLevel { get; init; }

    /// <summary>
    /// 出力用メッセージ
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    /// 優先度
    /// </summary>
    public required int Priority { get; init; }

    /// <summary>
    /// 検査項目ID_出力用
    /// フロントエンドでエラーメッセージを出す場所として渡します
    /// </summary>
    public int? ExamItemId { get; set; }
}

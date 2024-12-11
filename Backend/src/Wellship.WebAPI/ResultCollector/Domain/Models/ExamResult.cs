namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 検査結果を受診で束ねたクラス
/// </summary>
public class ExamResult
{
    /// <summary>
    /// 受診ID
    /// </summary>
    public required int ConsultId { get; init; }

    /// <summary>
    /// 検査項目明細単位の結果
    /// </summary>
    public required IEnumerable<ExamItemDetailResult> ExamItemDetailResults { get; init; }
}

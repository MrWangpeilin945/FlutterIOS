namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 過去検査結果を受診で束ねたクラス
/// </summary>
public class PreviousResult
{
    /// <summary>
    /// 受診ID
    /// </summary>
    public required int ConsultId { get; init; }

    /// <summary>
    /// 受診日
    /// </summary>
    public required DateOnly ExamDate { get; init; }

    /// <summary>
    /// 検査項目明細単位の過去結果
    /// </summary>
    public required IEnumerable<ExamItemDetailResult> ExamItemDetailResults { get; init; }
}

using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

/// <summary>
/// 検査結果リポジトリ
/// </summary>
public interface IResultRepository
{
    /// <summary>
    /// 検査結果を登録する
    /// </summary>
    public Task RegisterResultsAsync(Guid consultId, ExamResultRegisteEntity[] results);

    /// <summary>
    /// 検査結果登録時のログを記録する
    /// </summary>
    public Task WriteResultsLogAsync(Guid consultId, ExamResultRegisteEntity[] results);

    /// <summary>
    /// 受診を指定して複数の検査結果を取り消す
    /// </summary>
    /// <param name="consultId">受診ID</param>
    /// <param name="examItemDetailIds">削除対象の検査項目明細ID一覧</param>
    public Task BatchDeleteResultsAsync(Guid consultId, int[] examItemDetailIds);

    /// <summary>
    /// 全ての検査結果を取得する
    /// </summary>
    /// <param name="consultId">受診番号</param>
    public Task<IEnumerable<DisplayExamResultMenu>> GetConsultAllResults(Guid consultId);
}

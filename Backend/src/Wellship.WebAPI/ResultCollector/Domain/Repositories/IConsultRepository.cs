using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

/// <summary>
/// 受診リポジトリ
/// </summary>
public interface IConsultRepository
{
    /// <summary>
    /// 受診が存在するか
    /// </summary>
    /// <param name="consultNumber">受診番号</param>
    public Task<bool> ConsultExistsAsync(string consultNumber);

    /// <summary>
    /// 受診を取得します。
    /// </summary>
    public Task<Consult> GetConsultAsync(string consultNumber);

    /// <summary>
    /// 受診リストを取得します。
    /// </summary>
    public Task<IEnumerable<Consult>> GetConsultsAsync(string[] consultNumbers);

    /// <summary>
    /// 未受診の検査項目を受診単位のリストで取得します。
    /// </summary>
    public Task<IEnumerable<UnexaminedConsult>> GetUnexaminedConsultsAsync(string[] consultNumbers);

    /// <summary>
    /// 受診を指定して検査中止を取得します。
    /// </summary>
    public Task<ExamCancel> GetExamCancelsAsync(Guid consultId);

    /// <summary>
    /// 検査中止を削除・保存します。
    /// 実施で検査項目明細の中止が存在すれば削除します。
    /// すでに同じ検査項目明細の中止が存在すれば上書き更新、存在しなければ新規作成します。
    /// </summary>
    public Task SaveExamCancelsAsync(Guid consultId, int[] removeDetailIds, IEnumerable<ExamItemCancel> examItemCancels);

    /// <summary>
    /// 受診を指定して検査依頼を取得します。
    /// </summary>
    public Task<ExamOrder> GetExamOrdersAsync(Guid consultId);

    /// <summary>
    /// 受診を指定して検査結果を取得します。
    /// </summary>
    public Task<ExamResult> GetExamResultsAsync(Guid consultId);

    /// <summary>
    /// 受診を指定して過去検査結果を取得します。
    /// </summary>
    public Task<PreviousResult> GetPreviousResultsAsync(Guid consultId, DateOnly examDate);

    /// <summary>
    /// 受診を指定して検査項目特記を取得します。
    /// </summary>
    public Task<IEnumerable<ConsultNote>> GetConsultNotesAsync(Guid consultId);

    /// <summary>
    /// 検査基準値範囲を取得します。
    /// 受診に紐づく検査依頼に対して基準値を結合します。
    /// </summary>
    /// <param name="consultId">受診ID</param>
    /// <param name="examItemDetailIds">検査項目明細ID</param>
    Task<IEnumerable<ExamNormalValueRange>> GetExamNormalValueRangesAsync(Guid consultId, int[] examItemDetailIds);
}

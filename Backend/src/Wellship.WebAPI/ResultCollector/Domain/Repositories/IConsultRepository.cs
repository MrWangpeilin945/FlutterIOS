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
    public Task<ExamCancel> GetExamCancelsAsync(int consultId);

    /// <summary>
    /// 検査中止を削除します。
    /// </summary>
    public Task RemoveExamCancelsAsync(int consultId, int[] examItemDetailIds);

    /// <summary>
    /// 検査中止を保存します。
    /// すでに同じ検査項目明細の中止が存在すれば上書き更新、存在しなければ新規作成します。
    /// </summary>
    public Task SaveExamCancelsAsync(int consultId, IEnumerable<ExamItemCancel> examItemCancels);

    /// <summary>
    /// 基準値の基準値パターンIDを取得する
    /// </summary>
    /// <param name="consultId">受診ID</param>
    public Task<IEnumerable<int>> GetConsultThresholds(int consultId);

    /// <summary>
    /// 受診を指定して検査依頼を取得します。
    /// </summary>
    public Task<ExamOrder> GetExamOrdersAsync(int consultId);

    /// <summary>
    /// 受診を指定して検査結果を取得します。
    /// </summary>
    public Task<ExamResult> GetExamResultsAsync(int consultId);

    /// <summary>
    /// 受診を指定して過去検査結果を取得します。
    /// </summary>
    public Task<PreviousResult> GetPreviousResultsAsync(int consultId, DateOnly examDate);

}

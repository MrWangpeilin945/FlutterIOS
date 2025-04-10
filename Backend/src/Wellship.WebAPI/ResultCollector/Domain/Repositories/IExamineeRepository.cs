using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

/// <summary>
/// 受診者リポジトリ
/// </summary>
public interface IExamineeRepository
{
    /// <summary>
    /// 受診者IDで受診者を取得します。
    /// </summary>
    /// <param name="examineeId">受診者ID</param>
    public Task<Examinee> GetExamineeAsync(Guid examineeId);

    /// <summary>
    /// 会場日程ID,検査メニューID,進捗状況に該当する受診ID一覧を取得します。
    /// </summary>
    /// <param name="placeScheduleId">会場日程ID</param>
    /// <param name="examMenuId">検査メニューID</param>
    /// <param name="status">進捗状況</param>
    public Task<IEnumerable<Guid>> GetConsultIdsAsync(Guid placeScheduleId, int examMenuId, AggregatedProgressStatus status);

    /// <summary>
    /// 受診ID配列で受診者一覧を取得します。
    /// </summary>
    /// <param name="consultIds">受診ID配列</param>
    public Task<IEnumerable<ConsultExaminee>> GetConsultExamineesAsync(Guid[] consultIds);
}

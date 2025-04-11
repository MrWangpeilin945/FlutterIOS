using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;
using Ryobi.Wellship.Core.Enums;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

/// <summary>
/// 進捗リポジトリ
/// </summary>
public interface IProgressRepository
{

    /// <summary>
    /// 会場日程IDを指定して検査メニューごとの進捗状況を取得します。
    /// </summary>
    public Task<AggregatedProgress> GetAggregatedProgressAsync(Guid placeScheduleId);

    /// <summary>
    /// 会場日程IDを指定して検査メニューごとの進捗状況を取得します。
    /// </summary>
    public Task<AggregatedProgressDetail> GetAggregatedProgressByStatusAsync(Guid placeScheduleId, int? examMenuId);
}

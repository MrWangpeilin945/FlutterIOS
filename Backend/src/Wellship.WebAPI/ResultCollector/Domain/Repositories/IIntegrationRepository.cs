using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

/// <summary>
/// 基幹システム連携リポジトリ
/// </summary>
public interface IIntegrationRepository
{
    /// <summary>
    /// 検査結果出力履歴を取得する
    /// </summary>
    public Task<IEnumerable<ExportHistory>> GetExportHistoryAsync();

    /// <summary>
    /// 検査結果出力のために会場日程ごとの受診を取得する
    /// </summary>
    public Task<IEnumerable<ExportPlaceSchedule>> GetExportPlaceSchedulesAsync();

    /// <summary>
    /// 出力履歴IDを指定して受診の出力状況を未出力に戻す
    /// </summary>
    public Task UndoExportStatusAsync(Guid exportId);
}

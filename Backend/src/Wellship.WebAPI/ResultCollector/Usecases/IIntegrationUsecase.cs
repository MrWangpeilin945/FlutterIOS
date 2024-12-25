using Ryobi.Wellship.APIModels.Responses;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

/// <summary>
/// 基幹システム連携ユースケースのインターフェース
/// </summary>
public interface IIntegrationUsecase
{
    /// <summary>
    /// AP1018_連携対象の検査結果を取得する
    /// </summary>
    public Task<ExportDataList> GetExportTargetResultsAsync();

    /// <summary>
    /// AP1019_連携用に検査結果を出力する
    /// </summary>
    public Task ExportResultsAsync(Guid placeScheduleId);

    /// <summary>
    /// AP1020_検査結果の出力履歴を取得する
    /// </summary>
    public Task<ExportHistoryList> GetExportHistoryAsync();

    /// <summary>
    /// AP1021_出力した結果を未出力に戻す
    /// </summary>
    public Task UndoExportStatusAsync(Guid exportId);
}
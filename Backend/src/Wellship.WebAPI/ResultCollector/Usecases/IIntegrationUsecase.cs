using Ryobi.Wellship.APIModels.Responses;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

/// <summary>
/// 基幹システム連携ユースケースのインターフェース
/// </summary>
public interface IIntegrationUsecase
{
    /// <summary>
    /// 連携対象の検査結果を取得する
    /// </summary>
    public Task<ExportDataList> GetExportTargetResultsAsync();

    /// <summary>
    /// 連携用に検査結果を出力する
    /// </summary>
    public void ExportResults();

    /// <summary>
    /// 検査結果の出力履歴を取得する
    /// </summary>
    public Task<ExportHistoryList> GetExportHistoryAsync();
}
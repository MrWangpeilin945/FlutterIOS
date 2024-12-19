using Ryobi.Wellship.APIModels.Requests;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

/// <summary>
/// 検査結果ユースケースのインターフェース
/// </summary>
public interface IResultUsecase
{
    /// <summary>
    /// 検査結果を検証する
    /// </summary>
    public void VerifyResults();

    /// <summary>
    /// 検査結果を登録する
    /// </summary>
    public Task RegisterResultsAsync(string consultNumber, ResultsRequest results);
}
using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;
/// <summary>
/// EC2014_基準パターンを登録する インターフェース
/// </summary>
public interface IThresholdUsecase
{
    /// <summary>
    /// EC2014_基準パターンを登録する
    /// </summary>
    /// <param name="thresholds">基準パターン</param>
    /// <returns>エラーリスト</returns>
    public Task<List<ErrorObject>> StoreThresholdsAsync(List<Threshold> thresholds);
}

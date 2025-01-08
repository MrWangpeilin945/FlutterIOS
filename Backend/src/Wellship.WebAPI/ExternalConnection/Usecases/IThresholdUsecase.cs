using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Usecases
{
    /// <summary>
    /// 基準パターンを登録するUsecase層
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
}

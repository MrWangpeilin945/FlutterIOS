using System.ComponentModel;
using System.Text;

using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Usecases
{
    public class ThresholdUsecases
    {

        /// <summary>
        /// EC2014_基準パターンを登録する
        /// </summary>
        /// <param name="Thresholds"></param>
        /// <returns></returns>
        public async Task<List<ErrorObject>> StoreThresholdsAsync(List<Threshold> Thresholds)
        {
            return new List<ErrorObject> { };
        }
    }

}

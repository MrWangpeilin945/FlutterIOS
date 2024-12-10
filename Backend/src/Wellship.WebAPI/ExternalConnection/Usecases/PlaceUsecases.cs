using System.ComponentModel;
using System.Text;

using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Usecases
{
    public class PlaceUsecases
    {
        /// <summary>
        /// EC2007_会場を登録する
        /// </summary>
        /// <param name="Places"></param>
        /// <returns></returns>
        public async Task<List<ErrorObject>> StorePlacesAsync(List<Place> Places)
        {
            return new List<ErrorObject> { };

        }
    }

}

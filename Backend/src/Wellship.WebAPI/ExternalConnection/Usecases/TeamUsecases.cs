using System.ComponentModel;
using System.Text;

using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Usecases
{
    public class TeamUsecases
    {
        /// <summary>
        /// EC2006_班を登録する
        /// </summary>
        /// <param name="Teams"></param>
        /// <returns></returns>
        public async Task<List<ErrorObject>> StoreTeamsAsync(List<Team> Teams)
        {
            return new List<ErrorObject> { };

        }
    }

}

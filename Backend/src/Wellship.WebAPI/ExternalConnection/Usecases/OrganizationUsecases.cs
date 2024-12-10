using System.ComponentModel;
using System.Text;

using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Usecases
{
    /// <summary>
    /// EC2008_団体を登録する
    /// </summary>
    public class OrganizationUsecases
    {
        public async Task<List<ErrorObject>> StoreOrganizationsAsync(List<Organization> Organizations)
        {
            return new List<ErrorObject> { };
        }
    }

}

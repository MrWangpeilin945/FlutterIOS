using System.ComponentModel;
using System.Text;

using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Usecases
{
    public class Examinees
    {
        public async Task<List<ErrorObject>> StoreExamineesAsync(List<Examinee> Examinees)
        {
            return new List<ErrorObject> { };

        }
    }

}

using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
namespace Ryobi.Wellship.WebAPI.ExternalConnection.Usecases
{
    /// <summary>
    /// 職員ユースケースインターフェース
    /// </summary>
    public interface IStaffUsecase
    {
        /// <summary>
        /// 職員を登録する
        /// </summary>
        /// <param name="staffs"></param>
        /// <returns></returns>
        public Task<List<ErrorObject>> StroreStaffsAsync(List<Staff> staffs);
    }
}

using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;
/// <summary>
/// EC2011_職員ユースケース インターフェース
/// </summary>
public interface IStaffUsecase
{
    /// <summary>
    /// EC2011_職員を登録する
    /// </summary>
    /// <param name="staffs"></param>
    /// <returns></returns>
    public Task<List<ErrorObject>> StoreStaffsAsync(List<Staff> staffs);
}

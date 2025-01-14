using Ryobi.Wellship.Core.Exceptions;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Auth;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

/// <summary>
/// 職員ユースケース
/// </summary>
public class StaffUsecase : IStaffUsecase
{
    private readonly IStaffRepository _staffRepository;
    private readonly IStaffIdentityProvider _staffIdentityProvider;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="staffRepository">職員リポジトリ</param>
    /// <param name="staffIdentityProvider">職員情報プロバイダ</param>
    public StaffUsecase(IStaffRepository staffRepository, IStaffIdentityProvider staffIdentityProvider)
    {
        _staffRepository = staffRepository;
        _staffIdentityProvider = staffIdentityProvider;
    }

    /// <summary>
    /// AP1002_職員の情報を取得する
    /// </summary>
    public async Task<APIModels.Responses.Staff> GetStaffAsync()
    {
        var staffId = _staffIdentityProvider.StaffId;
        if (staffId is null)
        {
            throw new WellshipAuthenticationException();
        }

        var staff = await _staffRepository.GetStaffByStaffIdAsync((Guid)staffId);
        return new APIModels.Responses.Staff()
        {
            StaffId = staff.StaffId,
            StaffName = staff.Name
        };
    }
}

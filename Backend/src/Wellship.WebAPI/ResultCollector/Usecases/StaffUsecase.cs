using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

/// <summary>
/// 職員ユースケース
/// </summary>
public class StaffUsecase : IStaffUsecase
{
    private readonly IStaffRepository _staffRepository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="staffRepository">職員リポジトリ</param>
    public StaffUsecase(IStaffRepository staffRepository)
    {
        _staffRepository = staffRepository;
    }

    /// <summary>
    /// AP1002_職員の情報を取得する
    /// </summary>
    public async Task<APIModels.Responses.Staff> GetStaffAsync(Guid staffId)
    {
        var staff = await _staffRepository.GetStaffByStaffIdAsync(staffId);
        return new APIModels.Responses.Staff()
        {
            StaffId = staff.StaffId,
            StaffName = staff.Name
        };
    }
}

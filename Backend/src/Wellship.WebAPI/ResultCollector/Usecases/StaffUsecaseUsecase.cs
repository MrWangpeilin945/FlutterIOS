using Ryobi.Wellship.APIModels.Requests;
using Ryobi.Wellship.Core.Exceptions;
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
    /// 職員の情報を取得する
    /// </summary>
    public void GetStaff()
    {

    }
}

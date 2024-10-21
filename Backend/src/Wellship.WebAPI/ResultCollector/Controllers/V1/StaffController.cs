using Microsoft.AspNetCore.Mvc;

using Ryobi.Wellship.APIModels.Requests;
using Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Controllers.V1;

/// <summary>
/// 職員コントローラー
/// </summary>
[ApiController]
[ApiVersion("1")]
public class StaffController : ControllerBase
{
    private readonly IStaffUsecase _administratorUsecase;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="staffUsecaseUsecase">職員ユースケース</param>
    public StaffController(IStaffUsecase staffUsecaseUsecase)
    {
        _administratorUsecase = staffUsecaseUsecase;
    }

    /// <summary>
    /// 職員の情報を取得する
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("api/v{version:apiVersion}/staffs/{staffId}")]
    public IActionResult GetStaff()
    {
        _administratorUsecase.GetStaff();
        return Ok();
    }
}

using System.Text.Json;

using Asp.Versioning;

using Microsoft.AspNetCore.Mvc;

using NSwag.Annotations;

using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Controllers.V1;
/// <summary>
/// 職員 コントローラ
/// </summary>
[ApiController]
[ApiVersion("1")]
[OpenApiIgnore]
public class StaffController : ControllerBase
{
    private readonly IStaffUsecase _staffUsecase;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="staffUsecase">職員 ユースケース</param>
    public StaffController(IStaffUsecase staffUsecase)
    {
        _staffUsecase = staffUsecase;
    }

    /// <summary>
    /// EC2011_職員を登録する
    /// </summary>
    /// <param name="request">連携データ</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status207MultiStatus, Type = typeof(ErrorObject))]
    [HttpPost]
    [Route("api/v{version:apiVersion}/external/staffs")]
    public async Task<IActionResult> StoreStaffsAsync([FromBody] Staff[] request)
    {
        var result = await _staffUsecase.StoreStaffsAsync(request.ToList());

        if (result.Count > 0)
        {
            return new ContentResult
            {
                Content = JsonSerializer.Serialize(result),
                ContentType = "application/json",
                StatusCode = StatusCodes.Status207MultiStatus,
            };
        }
        else
        {
            return Ok();
        }
    }
}

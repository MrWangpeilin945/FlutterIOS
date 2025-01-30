using Asp.Versioning;

using Microsoft.AspNetCore.Mvc;

using NSwag.Annotations;

using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Controllers;

/// <summary>
/// 団体テスト用コントローラー
/// </summary>
[ApiController]
[ApiVersion("1")]
[OpenApiIgnore]
public class OrganizationtController : ControllerBase
{
    private readonly IOrganizationUsecase _organizationUsecases;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="organizationUsecases"></param>
    public OrganizationtController(IOrganizationUsecase organizationUsecases)
    {
        _organizationUsecases = organizationUsecases;
    }

    /// <summary>
    /// EC2008_団体を登録する
    /// </summary>
    /// <param name="request">連携データ</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ErrorObject))]
    [HttpPost]
    [Route("api/v{version:apiVersion}/external/organizations")]
    public async Task<IActionResult> StoreOrganizationsAsync([FromBody] Organization[] request)
    {
        var result = await _organizationUsecases.StoreOrganizationsAsync(request.ToList());

        return Ok(result);
    }
}

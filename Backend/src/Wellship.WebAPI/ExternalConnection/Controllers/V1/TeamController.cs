using Asp.Versioning;

using Microsoft.AspNetCore.Mvc;

using NSwag.Annotations;

using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Controllers.V1;
/// <summary>
/// 班 コントローラ
/// </summary>
[ApiController]
[ApiVersion("1")]
[OpenApiIgnore]
public class TeamController : ControllerBase
{
    private readonly ITeamUsecase _teamUsecaseUsecase;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="teamUsecaseUsecase">班 ユースケース</param>
    public TeamController(ITeamUsecase teamUsecaseUsecase)
    {
        _teamUsecaseUsecase = teamUsecaseUsecase;
    }

    /// <summary>
    /// EC2006_班を登録する
    /// </summary>
    /// <param name="request">連携データ</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ErrorObject))]
    [HttpPost]
    [Route("api/v{version:apiVersion}/external/teams")]
    public async Task<IActionResult> StoreTeamsAsync([FromBody] Team[] request)
    {
        var result = await _teamUsecaseUsecase.StoreTeamsAsync(request.ToList());

        return Ok(result);
    }
}

using Asp.Versioning;
using System.Text.Json;

using Microsoft.AspNetCore.Mvc;

using NSwag.Annotations;

using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Controllers.V1;
/// <summary>
/// 受診 コントローラ
/// </summary>
[ApiController]
[ApiVersion("1")]
[OpenApiIgnore]
public class ConsultController : ControllerBase
{
    private readonly IConsultUsecase _consultUsecase;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="consultUsecase">受診 ユースケース</param>
    public ConsultController(IConsultUsecase consultUsecase)
    {
        _consultUsecase = consultUsecase;
    }

    /// <summary>
    /// EC2004_受診を更新する
    /// </summary>
    /// <param name="request">連携データ</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status207MultiStatus, Type = typeof(ErrorObject))]
    [HttpPost]
    [Route("api/v{version:apiVersion}/external/consult")]
    public async Task<IActionResult> StoreConsultAsync([FromBody] Consult[] request)
    {
        var result = await _consultUsecase.StoreConsultAsync(request.ToList());

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

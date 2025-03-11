using Asp.Versioning;
using System.Text.Json;

using Microsoft.AspNetCore.Mvc;

using NSwag.Annotations;

using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Controllers.V1;
/// <summary>
/// 受診者 コントローラ
/// </summary>
[ApiController]
[ApiVersion("1")]
[OpenApiIgnore]
public class ExamineeController : ControllerBase
{
    private readonly IExamineeUsecase _examineeUsecases;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="examineeUsecases">受診者 ユースケース</param>
    public ExamineeController(IExamineeUsecase examineeUsecases)
    {
        _examineeUsecases = examineeUsecases;
    }

    /// <summary>
    /// EC2001_受診者を登録する
    /// </summary>
    /// <param name="request">連携データ</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status207MultiStatus, Type = typeof(ErrorObject))]
    [HttpPost]
    [Route("api/v{version:apiVersion}/external/examinees")]
    public async Task<IActionResult> StoreExamineesAsync([FromBody] Examinee[] request)
    {
        var result = await _examineeUsecases.StoreExamineesAsync(request.ToList());

        if (result.Any())
        {
            return new ContentResult
            {
                Content = JsonSerializer.Serialize(result),
                ContentType = "application/json",
                StatusCode = (int)StatusCodes.Status207MultiStatus,
            };
        }
        else
        {
            return Ok();
        }
    }
}

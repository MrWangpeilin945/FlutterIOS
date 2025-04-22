using System.Text.Json;

using Asp.Versioning;

using Microsoft.AspNetCore.Mvc;

using NSwag.Annotations;

using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Controllers.V1;
/// <summary>
/// 基準値(範囲) コントローラ
/// </summary>
[ApiController]
[ApiVersion("1")]
[OpenApiIgnore]
public class ExamNormalValueRangeController : ControllerBase
{
    private readonly IExamNormalValueRangeUsecase _examNormalValueRange;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="examNormalValueRange">基準値範囲 ユースケース</param>
    public ExamNormalValueRangeController(IExamNormalValueRangeUsecase examNormalValueRange)
    {
        _examNormalValueRange = examNormalValueRange;
    }

    /// <summary>
    /// EC2009_基準値(範囲)を登録する
    /// </summary>
    /// <param name="request">連携データ</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status207MultiStatus, Type = typeof(ErrorObject))]
    [HttpPost]
    [Route("api/v{version:apiVersion}/external/examNormalValueRange")]
    public async Task<IActionResult> StoreExamNormalValueRangeAsync([FromBody] ExamNormalValueRange[] request)
    {
        var result = await _examNormalValueRange.StoreExamNormalValueRangeAsync(request.ToList());

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

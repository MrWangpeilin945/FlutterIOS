using Asp.Versioning;

using Microsoft.AspNetCore.Mvc;

using NSwag.Annotations;

using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Controllers.V1;
/// <summary>
/// 受診者テスト用コントローラー
/// </summary>
[ApiController]
[ApiVersion("1")]
[OpenApiIgnore]
public class ExamineeController : ControllerBase
{
    private readonly IExamineeUsecase _examineeUsecases;

    /// <summary>
    /// コントローラーを生成します。
    /// </summary>
    /// <param name="examineeUsecases">受診者ユースケース</param>
    public ExamineeController(IExamineeUsecase examineeUsecases)
    {
        _examineeUsecases = examineeUsecases;
    }

    /// <summary>
    /// EC2001_受診者を登録する
    /// </summary>
    /// <param name="request">連携データ</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ErrorObject))]
    [HttpPost]
    [Route("api/v{version:apiVersion}/external/examinees")]
    public async Task<IActionResult> StoreExamineesAsync([FromBody] Examinee[] request)
    {
        var result = await _examineeUsecases.StoreExamineesAsync(request.ToList());

        return Ok(result);
    }
}

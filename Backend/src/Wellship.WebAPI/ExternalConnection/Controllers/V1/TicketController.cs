using Asp.Versioning;

using Microsoft.AspNetCore.Mvc;

using NSwag.Annotations;

using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Controllers.V1;
/// <summary>
/// 受付 コントローラ
/// </summary>
[ApiController]
[ApiVersion("1")]
[OpenApiIgnore]
public class TicketController : ControllerBase
{
    private readonly ITicketUsecase _ticketUsecase;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="ticketUsecase">受付 ユースケース</param>
    public TicketController(ITicketUsecase ticketUsecase)
    {
        _ticketUsecase = ticketUsecase;
    }

    /// <summary>
    /// EC2002_受付を更新する
    /// </summary>
    /// <param name="request">連携データ</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ErrorObject))]
    [HttpPost]
    [Route("api/v{version:apiVersion}/external/tickets")]
    public async Task<IActionResult> StoreTicketsAsync([FromBody] Ticket[] request)
    {
        var result = await _ticketUsecase.StoreTicketsAsync(request.ToList());

        return Ok(result);
    }
}

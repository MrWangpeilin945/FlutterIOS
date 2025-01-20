using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

using Microsoft.AspNetCore.Mvc;

using NSwag.Annotations;

using Ryobi.Wellship.ExternalConnection.Enums;
using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.Usecases;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Controllers.V1;
/// <summary>
/// 動作確認用コントローラ
/// </summary>
[OpenApiIgnore]
public class TicketTestController : ControllerBase
{
    private readonly ITicketUsecase _ticketUsecase;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="ticketUsecase">受付ユースケース</param>
    public TicketTestController(ITicketUsecase ticketUsecase)
    {
        _ticketUsecase = ticketUsecase;
    }

    /// <summary>
    /// 受付を更新するテスト
    /// </summary>
    [HttpPost]
    [Route("api/v{version:apiVersion}/ec2002/ticket")]
    public async Task<IActionResult> StoreTicketsAsync([FromQuery][Required] int actionType)
    {
        // 10000～1009999までの連携キーを3つずつ作成する
        var tickets = (from x in Enumerable.Range(10000, 10000)
                       from y in Enumerable.Range(1, 3)
                       select new Ticket
                       {
                           SortNo = (x - 10000) * 3 + y,
                           ConnectionCode = x.ToString(),
                           ActionType = (ActionType)actionType,
                           TicketNumber = (x + y).ToString(),
                           InputNote = ((x - 10000) * 3 + y).ToString()
                       }).ToList();

        var stopwatch = Stopwatch.StartNew();
        stopwatch.Start();

        var result = await _ticketUsecase.StoreTicketsAsync(tickets);

        var processingTime = stopwatch.Elapsed.TotalSeconds;
        return Ok(new
        {
            DataProcessingTime = processingTime.ToString() + "秒",
            ErrorObject = result
        });
    }
}

using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Ryobi.Wellship.WebAPI.ExternalConnection.Usecases;
using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Controllers.V1;
/// <summary>
/// 動作確認用コントローラ
/// </summary>
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
    public async Task<IActionResult> StoreTicketsAsync()
    {
        // 10000～10099までの連携キーを3つずつ作成する
        var tickets = (from x in Enumerable.Range(10000, 100)
                        from y in Enumerable.Range(1,3)
                        select new Ticket
                        {
                            SortNo =  (x - 10000) * 3 + y,
                            ConnectionCode = x.ToString(),
                            ActionType = x % 2 == 0 ? Wellship.ExternalConnection.Enums.ActionType.登録 : Wellship.ExternalConnection.Enums.ActionType.削除,
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

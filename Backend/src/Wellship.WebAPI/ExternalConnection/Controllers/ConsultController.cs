using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Ryobi.Wellship.WebAPI.ExternalConnection.Usecases;
using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.ExternalConnection.Enums;
using System.Net;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Controllers.V1;
/// <summary>
/// 動作確認用コントローラ
/// </summary>
public class ConsultTestController : ControllerBase
{
    private readonly IConsultUsecase _consultUsecase;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="consultUsecase">受診ユースケース</param>
    public ConsultTestController(IConsultUsecase consultUsecase)
    {
        _consultUsecase = consultUsecase;
    }

    /// <summary>
    /// 受診を更新するテスト
    /// </summary>
    [HttpPost]
    [Route("api/v{version:apiVersion}/ec2004/consult")]
    public async Task<IActionResult> StoreConsultAsync([FromQuery][Required] int actionType)
    {
        // 10000～1009999までの連携キーを3つずつ作成する
        /*
        var consults = (from x in Enumerable.Range(10000, 10000)
                        from y in Enumerable.Range(1,3)
                        select new Consult
                        {
                            SortNo =  (x - 10000) * 3 + y,
                            ConnectionCode = x.ToString(),
                            ActionType = (ActionType)actionType,
                            TicketNumber = (x + y).ToString(),
                            InputNote = ((x - 10000) * 3 + y).ToString()
                        }).ToList();
        */
        var consults = new List<Consult>{
            new Consult{
                SortNo = 1,                                 // 処理順
                ConnectionCode = "10001",                   // 連携キー
                ActionType = (ActionType)actionType,        // 連携モード
                PlaceCode = "P001",                         // 会場コード
                TeamCode= "T001",                           // 班コード
                ExamDate = DateOnly.Parse("2024/10/02"),    // 健診日
                ConsultNumber = "0001",                     // 受診番号
                ExamineeCd = "100002",                      // 受診者コード
                Note = "",                                  // 受診.特記事項
                PreviousResults = [new PreviousResult{      // 過去検査結果
                    ExamItemDetailCd = "1192",                  // 検査項目明細CD
                    ExamDate = DateOnly.Parse("2024/10/02"),    // 検査日
                    Value = "100"                               // 結果値
                }],
                ConsultThresholds = [new ConsultThreshold{  //基準値判定
                    ThresholdCode = "ryobi",                    // 基準値判定コード
                    Priority =1                                 // 優先度
                }],
                ConsultNotes = [new ConsultNote{            // 受診特記.特記事項
                    Code = "ST01",                              // 検査特記コード
                    Note = "撮影番号"                           // 特記事項
                }],                          
                ExamItemDetailOrders = [new ExamItemDetailOrder{    // 検査項目明細依頼
                    ExamItemDetailCd = "1192"
                }],
                InputNote = "20250110"
            }
        };

        var stopwatch = Stopwatch.StartNew();
        stopwatch.Start();

        var result = await _consultUsecase.StoreConsultAsync(consults);

        var processingTime = stopwatch.Elapsed.TotalSeconds;
        return Ok(new
        {
            DataProcessingTime = processingTime.ToString() + "秒",
            ErrorObject = result
        });
    }
}

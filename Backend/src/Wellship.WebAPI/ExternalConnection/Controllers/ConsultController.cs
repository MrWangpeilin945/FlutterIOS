using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Ryobi.Wellship.WebAPI.ExternalConnection.Usecases;
using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.ExternalConnection.Enums;
using System.Text.Json.Serialization;

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
    /// <param name="actionType">連携モード</param>
    /// <param name="request">連携データ</param>
    /// <returns></returns>
    [HttpPost]
    [Route("api/v{version:apiVersion}/ec2004/consult")]
    public async Task<IActionResult> StoreConsultAsync([FromQuery] int? actionType, [FromBody] Consult[] request)
    {
        List<Consult> consults;
        if(actionType != null)
        {
            // 100001～110000まで外部連携キーを作成する
            consults = (from x in Enumerable.Range(100001, 10000)
                            select new Consult
                            {
                                SortNo =  x - 100000,                       // 処理順
                                ConnectionCode = x.ToString(),              // 連携キー
                                ActionType = (ActionType)actionType,        // 連携モード
                                PlaceCode = "P001",                         // 会場コード
                                TeamCode= "T001",                           // 班コード
                                ExamDate = DateOnly.Parse("2024/10/02"),    // 健診日
                                ConsultNumber = x.ToString(),               // 受診番号
                                ExamineeCd = "100002",                      // 受診者コード
                                Note = "特記事項" + x.ToString(),            // 受診.特記事項
                                PreviousResults = [new PreviousResult{      // 過去検査結果
                                    ExamItemDetailCd = "Previous5963",         // 検査項目明細CD
                                    ExamDate = DateOnly.Parse("2024/10/02"),   // 検査日
                                    Value = x.ToString()                       // 結果値
                                }],
                                ConsultThresholds = [new ConsultThreshold{  //基準値判定
                                    ThresholdCode = "ryobi",                    // 基準値判定コード
                                    Priority = x - 100000                       // 優先度
                                }],
                                ConsultNotes = [new ConsultNote{            // 受診特記.特記事項
                                    Code = "ST01",                              // 検査特記コード
                                    Note = "撮影番号" + x.ToString()             // 特記事項
                                }],
                                ExamItemDetailOrders = [new ExamItemDetailOrder{    // 検査項目明細依頼
                                    ExamItemDetailCd = "Order8931"                      // 検査項目明細CD
                                }],
                                InputNote = "InputNote" + x.ToString()
                            }).ToList();
        }
        else
        {
            // リクエストから受診を更新する
            consults = request.ToList();
        }

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

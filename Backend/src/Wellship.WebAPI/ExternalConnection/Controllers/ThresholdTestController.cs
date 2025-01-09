using Microsoft.AspNetCore.Mvc;

using Ryobi.Wellship.WebAPI.ExternalConnection.Usecases;
using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Controllers.V1
{
    /// <summary>
    /// 動作確認用コントローラ
    /// </summary>
    public class ThresholdTestController : ControllerBase
    {
        private readonly IThresholdUsecase _administratorUsecase;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="thresholdUsecaseUsecase">基準パターンユースケース</param>
        public ThresholdTestController(IThresholdUsecase thresholdUsecaseUsecase)
        {
            _administratorUsecase = thresholdUsecaseUsecase;
        }

        /// <summary>
        /// 基準パターン
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Threshold))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("api/v{version:apiVersion}/threshold/profile")]
        public async Task<IActionResult> TestThresholdAsync()
        {
            // 投入データ作成
            List<Threshold> thresholds;
            thresholds = CreateTestThresholds(10000);
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            var result = await _administratorUsecase.StoreThresholdsAsync(thresholds);
            sw.Stop();
            TimeSpan ts = sw.Elapsed;
            Console.WriteLine($"　{ts.Hours}時間 {ts.Minutes}分 {ts.Seconds}秒 {ts.Milliseconds}ミリ秒");
            return Ok(result);
        }

        /// <summary>
        /// 基準パターン（Upsertテスト）
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Threshold))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("api/v{version:apiVersion}/threshold_upsert/profile")]
        public async Task<IActionResult> TestThresholdUpsertAsync()
        {
            // 投入データ作成
            List<Threshold> thresholds;
            thresholds = CreateUpsertTestThresholds();
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            var result = await _administratorUsecase.StoreThresholdsAsync(thresholds);
            sw.Stop();
            TimeSpan ts = sw.Elapsed;
            Console.WriteLine($"　{ts.Hours}時間 {ts.Minutes}分 {ts.Seconds}秒 {ts.Milliseconds}ミリ秒");
            return Ok(result);
        }
        
        /// <summary>
        /// 投入データ作成
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        protected List<Threshold> CreateTestThresholds(int num)
        {
            List<Threshold> thresholds = new List<Threshold>();
            for(int i = 1; i < num + 1; i++)
            {
                Threshold tmp_threshold = new Threshold{Code="Threshold"+i.ToString("00000"), Name="基準パターン"+i.ToString("00000"), InputNote=i.ToString() };
                thresholds.Add((Threshold)tmp_threshold);
            }
            return thresholds;
        }
        
        /// <summary>
        /// 投入データ作成
        /// </summary>
        /// <returns></returns>
        protected List<Threshold> CreateUpsertTestThresholds()
        {
            List<Threshold> thresholds = new List<Threshold>();
            thresholds.Add(new Threshold{ Code="Threshold00010", Name="基準パターンupsert00010", InputNote=999.ToString() });
            return thresholds;
        }
        
    }
}

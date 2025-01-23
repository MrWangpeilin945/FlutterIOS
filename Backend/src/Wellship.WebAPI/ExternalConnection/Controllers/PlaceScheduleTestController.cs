using System.Diagnostics;

using Microsoft.AspNetCore.Mvc;

using Asp.Versioning;

using NSwag.Annotations;

using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.Usecases;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Controllers
{
    /// <summary>
    /// 会場日程テスト用コントローラー
    /// </summary>
    [ApiController]
    [ApiVersion("1")]
    [OpenApiIgnore]
    public class PlaceScheduleTestController : ControllerBase
    {
        private readonly IPlaceScheduleUsecase _placeScheduleUsecase;

        /// <summary>
        /// コントローラーの生成
        /// </summary>
        /// <param name="placeScheduleUsecases">会場日程コントローラー</param>
        public PlaceScheduleTestController(IPlaceScheduleUsecase placeScheduleUsecases)
        {
            _placeScheduleUsecase = placeScheduleUsecases;
        }

        /// <summary>
        /// 会場日程登録テスト（Upsertテスト）
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v{version:apiVersion}/place_schedule/test1")]
        public async Task<IActionResult> PlaceScheduleTest1Async()
        {
            var stopwatch = Stopwatch.StartNew();
            // 投入データ作成
            List<PlaceSchedule> placeSchedules;
            placeSchedules = CreateTestPlaceSchedules(10000);
            stopwatch.Stop();
            var creationTime = stopwatch.Elapsed.TotalSeconds;
            stopwatch.Restart();
            var result = await _placeScheduleUsecase.StorePlaceSchedulesAsync(placeSchedules);
            var processingTime = stopwatch.Elapsed.TotalSeconds;
            return Ok(new
            {
                DataCreationTime = creationTime.ToString() + "秒",
                DataProcessingTime = processingTime.ToString() + "秒",
                ErrorObject = result
            });
        }

        /// <summary>
        /// 投入データ作成
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        protected List<PlaceSchedule> CreateTestPlaceSchedules(int num)
        {
            List<PlaceSchedule> placeSchedules = new List<PlaceSchedule>();
            for (int i = 1; i < num + 1; i++)
            {
                PlaceSchedule tmp_place_schedule = new PlaceSchedule { TeamCode = "Team" + i.ToString("00000"), PlaceCode = "Place" + i.ToString("00000"), ExamDate = DateTime.Parse("2025/1/1 12:34:00"), InputNote = i.ToString() };
                placeSchedules.Add((PlaceSchedule)tmp_place_schedule);
            }
            return placeSchedules;
        }

        /// <summary>
        /// EC2012_会場日程を登録する
        /// </summary>
        /// <param name="request">連携データ</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v{version:apiVersion}/ec2012/placeSchedule")]
        public async Task<IActionResult> StorePlaceSchedulesAsync([FromBody] PlaceSchedule[] request)
        {
            var stopwatch = Stopwatch.StartNew();
            stopwatch.Start();

            var result = await _placeScheduleUsecase.StorePlaceSchedulesAsync(request.ToList());

            var processingTime = stopwatch.Elapsed.TotalSeconds;
            return Ok(new
            {
                DataProcessingTime = processingTime.ToString() + "秒",
                ErrorObject = result
            });
        }
    }
}

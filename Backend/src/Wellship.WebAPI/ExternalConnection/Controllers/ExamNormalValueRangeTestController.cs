using System.Diagnostics;

using Microsoft.AspNetCore.Mvc;

using Asp.Versioning;

using NSwag.Annotations;

using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Controllers
{
    /// <summary>
    /// 基準値範囲テスト用コントローラー
    /// </summary>
    [ApiController]
    [ApiVersion("1")]
    [OpenApiIgnore]
    public class ExamNormalValueRangeTestController : ControllerBase
    {
        private readonly IExamNormalValueRangeUsecase _examNormalValueRange;

        /// <summary>
        /// コントローラーの生成
        /// </summary>
        /// <param name="examNormalValueRange">基準値範囲コントローラー</param>
        public ExamNormalValueRangeTestController(IExamNormalValueRangeUsecase examNormalValueRange)
        {
            _examNormalValueRange = examNormalValueRange;
        }

        /// <summary>
        /// 基準値範囲登録テスト（Upsertテスト）
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v{version:apiVersion}/exam_normal_value_range/test1")]
        public async Task<IActionResult> ExamNormalValueRangeTest1Async()
        {
            var stopwatch = Stopwatch.StartNew();
            // 投入データ作成
            List<ExamNormalValueRange> examNormalValueRanges;
            examNormalValueRanges = CreateTestExamNormalValueRanges(10000);
            stopwatch.Stop();
            var creationTime = stopwatch.Elapsed.TotalSeconds;
            stopwatch.Restart();
            var result = await _examNormalValueRange.StoreExamNormalValueRangeAsync(examNormalValueRanges);
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
        protected List<ExamNormalValueRange> CreateTestExamNormalValueRanges(int num)
        {
            List<ExamNormalValueRange> examNormalValueRanges = new List<ExamNormalValueRange>();
            for (int i = 1; i < num + 1; i++)
            {
                ExamNormalValueRange tmp_exam_normal_value_range = new ExamNormalValueRange { Name = "Name" + i.ToString("00000"), ThresholdCd = "Threshold" + i.ToString("00000"), ExamItemDetailCd = "1", MaxAge = (9999999 - i).ToString(), MinAge = (0 + i).ToString(), TargetSex = (TargetSexType)3, MaxValue = decimal.Parse("9999"), MinValue = decimal.Parse("0.0"), ErrorLevel = (InputErrorLevel)1, InputNote = i.ToString() };
                examNormalValueRanges.Add((ExamNormalValueRange)tmp_exam_normal_value_range);
            }

            return examNormalValueRanges;
        }

        /// <summary>
        /// EC2009_基準値(範囲)を登録する
        /// </summary>
        /// <param name="request">連携データ</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v{version:apiVersion}/ec2009/examNormalValueRange")]
        public async Task<IActionResult> StoreExamNormalValueRangeAsync([FromBody] ExamNormalValueRange[] request)
        {
            var stopwatch = Stopwatch.StartNew();
            stopwatch.Start();

            var result = await _examNormalValueRange.StoreExamNormalValueRangeAsync(request.ToList());

            var processingTime = stopwatch.Elapsed.TotalSeconds;
            return Ok(new
            {
                DataProcessingTime = processingTime.ToString() + "秒",
                ErrorObject = result
            });
        }
    }
}

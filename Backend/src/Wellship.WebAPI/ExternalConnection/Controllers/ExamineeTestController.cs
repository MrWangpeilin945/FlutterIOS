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
    /// 受診者テスト用コントローラー
    /// </summary>
    [ApiController]
    [ApiVersion("1")]
    [OpenApiIgnore]
    public class ExamineeTestController : ControllerBase
    {
        private readonly IExamineeUsecase _examineeUsecases;

        /// <summary>
        /// コントローラーを生成します。
        /// </summary>
        /// <param name="examineeUsecases">受診者ユースケース</param>
        public ExamineeTestController(IExamineeUsecase examineeUsecases)
        {
            _examineeUsecases = examineeUsecases;
        }

        /// <summary>
        /// テストケース1 新規受診者データの登録
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("api/v{version:apiVersion}/examinee/test1")]
        public async Task<IActionResult> ExamineeTest1Async()
        {
            // テストデータを生成
            var stopwatch = Stopwatch.StartNew();

            var examineeList = Enumerable.Range(200001, 10000).Select(i => new Examinee
            {
                ExamineeCode = i.ToString(),
                Name = "テスト　受診者" + i.ToString(),
                KanaName = "テスト　ジュシンシャ" + i.ToString(),
                Sex = Sex.男,
                Birthdate = DateTime.Parse($"2000-01-01 00:00:00"),
                Affiliations = Enumerable.Range(11, 3).Select(j => new Affiliation
                {
                    OrganizationCode = j.ToString()
                }).ToList(),
                InputNote = i.ToString()
            }).ToList();

            stopwatch.Stop();
            var creationTime = stopwatch.Elapsed.TotalSeconds;

            stopwatch.Restart();
            var result = await _examineeUsecases.StoreExamineesAsync(examineeList);

            var processingTime = stopwatch.Elapsed.TotalSeconds;

            return Ok(new
            {
                DataCreationTime = creationTime.ToString() + "秒",
                DataProcessingTime = processingTime.ToString() + "秒",
                ErrorObject = result
            });
        }

        /// <summary>
        /// テストケース2 既存受診者データの更新
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("api/v{version:apiVersion}/examinee/test2")]
        public async Task<IActionResult> ExamineeTest2Async()
        {
            // テストデータを生成
            var stopwatch = Stopwatch.StartNew();

            var examineeList = Enumerable.Range(200001, 1).Select(i => new Examinee
            {
                ExamineeCode = i.ToString(),
                Name = "更新テスト　受診者" + i.ToString(),
                KanaName = "コウシンテスト　ジュシンシャ" + i.ToString(),
                Sex = Sex.男,
                Birthdate = DateTime.Parse($"2000-01-02 00:00:00"),
                Affiliations = Enumerable.Range(11, 1).Select(j => new Affiliation
                {
                    OrganizationCode = j.ToString()
                }).ToList(),
                InputNote = i.ToString()
            }).ToList();

            stopwatch.Stop();
            var creationTime = stopwatch.Elapsed.TotalSeconds;

            stopwatch.Restart();
            var result = await _examineeUsecases.StoreExamineesAsync(examineeList);

            var processingTime = stopwatch.Elapsed.TotalSeconds;

            return Ok(new
            {
                DataCreationTime = creationTime.ToString() + "秒",
                DataProcessingTime = processingTime.ToString() + "秒",
                ErrorObject = result
            });
        }

        /// <summary>
        /// テストケース3 受診者データに紐づく団体情報が存在しない
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("api/v{version:apiVersion}/examinee/test3")]
        public async Task<IActionResult> ExamineeTest3Async()
        {
            // テストデータを生成
            var stopwatch = Stopwatch.StartNew();

            var examineeList = Enumerable.Range(200001, 1).Select(i => new Examinee
            {
                ExamineeCode = i.ToString(),
                Name = "テスト　受診者" + i.ToString(),
                KanaName = "テスト　ジュシンシャ" + i.ToString(),
                Sex = Sex.男,
                Birthdate = DateTime.Parse($"2000-01-01 00:00:00"),
                Affiliations = Enumerable.Range(11, 4).Select(j => new Affiliation
                {
                    OrganizationCode = j.ToString()
                }).ToList(),
                InputNote = i.ToString()
            }).ToList();

            stopwatch.Stop();
            var creationTime = stopwatch.Elapsed.TotalSeconds;

            stopwatch.Restart();
            var result = await _examineeUsecases.StoreExamineesAsync(examineeList);

            var processingTime = stopwatch.Elapsed.TotalSeconds;

            return Ok(new
            {
                DataCreationTime = creationTime.ToString() + "秒",
                DataProcessingTime = processingTime.ToString() + "秒",
                ErrorObject = result
            });
        }

        /// <summary>
        /// EC2001_受診者を登録する
        /// </summary>
        /// <param name="request">連携データ</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v{version:apiVersion}/ec2001/examinee")]
        public async Task<IActionResult> StoreConsultAsync([FromBody] Examinee[] request)
        {
            var stopwatch = Stopwatch.StartNew();
            stopwatch.Start();

            var result = await _examineeUsecases.StoreExamineesAsync(request.ToList());

            var processingTime = stopwatch.Elapsed.TotalSeconds;
            return Ok(new
            {
                DataProcessingTime = processingTime.ToString() + "秒",
                ErrorObject = result
            });
        }
    }
}

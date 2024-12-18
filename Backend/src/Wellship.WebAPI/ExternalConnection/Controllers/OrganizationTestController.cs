using System.Diagnostics;

using Microsoft.AspNetCore.Mvc;

using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.Usecases;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Controllers
{
    /// <summary>
    /// 団体テスト用コントローラー
    /// </summary>
    [ApiController]
    [ApiVersion("1")]
    public class OrganizationTestController : ControllerBase
    {
        private readonly IOrganizationUsecases _organizationUsecases;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="organizationUsecases"></param>
        public OrganizationTestController(IOrganizationUsecases organizationUsecases)
        {
            _organizationUsecases = organizationUsecases;
        }

        /// <summary>
        /// 団体を登録する
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("api/v{version:apiVersion}/organization/insert")]
        public async Task<IActionResult> InsertOrganizationAsync()
        {
            // テストデータを生成
            var stopwatch = Stopwatch.StartNew();

            var organizationList = Enumerable.Range(101, 10000)
                .Select(i => new Organization { Code = (i * 10).ToString(), Name = "テスト更新団体" + i.ToString() })
                .ToList();

            stopwatch.Stop();
            var creationTime = stopwatch.Elapsed.TotalSeconds;

            stopwatch.Restart();
            var result = await _organizationUsecases.StoreOrganizationsAsync(organizationList);
            stopwatch.Stop();

            var processingTime = stopwatch.Elapsed.TotalSeconds;

            return Ok(new
            {
                DataCreationTime = creationTime.ToString() + "秒",
                DataProcessingTime = processingTime.ToString() + "秒"
            });

        }
    }
}

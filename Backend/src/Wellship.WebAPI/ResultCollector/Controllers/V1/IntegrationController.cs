using Microsoft.AspNetCore.Mvc;

using Ryobi.Wellship.APIModels.Responses;
using Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Controllers.V1;

/// <summary>
/// 基幹システム連携コントローラー
/// </summary>
[ApiController]
[ApiVersion("1")]
public class IntegrationController : ControllerBase
{
    private readonly IIntegrationUsecase _integrationUsecase;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="integrationUsecase">基幹システム連携ユースケース</param>
    public IntegrationController(IIntegrationUsecase integrationUsecase)
    {
        _integrationUsecase = integrationUsecase;
    }

    /// <summary>
    /// 連携対象の検査結果を取得する
    /// </summary>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExportDataList))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet]
    [Route("api/v{version:apiVersion}/integrations/examResults")]
    public IActionResult GetIntegrationResults()
    {
        _integrationUsecase.GetIntegrationResults();
        return Ok();
    }

    /// <summary>
    /// 連携用に検査結果を出力する
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Route("api/v{version:apiVersion}/integrations/results/{placeScheduleId}/export")]
    public IActionResult ExportResults()
    {
        _integrationUsecase.ExportResults();
        return Ok();
    }

    /// <summary>
    /// 検査結果の出力履歴を取得する
    /// </summary>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExportHistoryList))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet]
    [Route("api/v{version:apiVersion}/integrations/examResults/exportHistory")]
    public IActionResult GetExportHistory()
    {
        _integrationUsecase.GetExportHistory();
        return Ok();
    }

}

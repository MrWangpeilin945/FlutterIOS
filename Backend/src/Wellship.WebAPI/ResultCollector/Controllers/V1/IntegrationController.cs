using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

using Ryobi.Wellship.APIModels.Requests;

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
    public async Task<IActionResult> GetIntegrationResults()
    {
        var result = await _integrationUsecase.GetExportTargetResultsAsync();
        return Ok(result);
    }

    /// <summary>
    /// 連携用に検査結果を出力する
    /// </summary>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPost]
    [Route("api/v{version:apiVersion}/integrations/examResults/{placeScheduleId}/export")]
    public async Task<IActionResult> ExportResults([FromRoute][Required] Guid placeScheduleId, [FromBody] ResultExportRequest request)
    {
        if (placeScheduleId != request.PlaceScheduleId)
        {
            return BadRequest("パスパラメータとリクエストボディ内の値が一致しません。");
        }

        await _integrationUsecase.ExportResultsAsync(placeScheduleId);
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
    public async Task<IActionResult> GetExportHistory()
    {
        var result = await _integrationUsecase.GetExportHistoryAsync();
        return Ok(result);
    }

    /// <summary>
    /// 出力した結果を未出力に戻す
    /// </summary>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost]
    [Route("api/v{version:apiVersion}/integrations/examResults/undoExport")]
    public async Task<IActionResult> UndoExportStatusAsync([FromBody] UndoIntegrationExportStatusRequest request)
    {
        await _integrationUsecase.UndoExportStatusAsync(request.ExportId);
        return Ok();
    }
}

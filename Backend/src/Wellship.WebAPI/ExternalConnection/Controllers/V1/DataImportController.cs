using Asp.Versioning;

using Microsoft.AspNetCore.Mvc;

using NSwag.Annotations;

using Ryobi.Wellship.APIModels.Requests;
using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.BackgroundTasks;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Controllers.V1;
/// <summary>
/// ファイル取り込み コントローラ
/// </summary>
[ApiController]
[ApiVersion("1")]
[OpenApiIgnore]
public class DataImportController : ControllerBase
{
    private readonly IServiceScopeTaskRunner _serviceScopeTaskRunner;


    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="serviceScopeTaskRunner"></param>
    public DataImportController(IServiceScopeTaskRunner serviceScopeTaskRunner)
    {
        _serviceScopeTaskRunner = serviceScopeTaskRunner;

    }

    /// <summary>
    /// EC1001_ファイル取り込みを実行する_随時
    /// </summary>
    /// <param name="s3EventRequest">S3イベントのリクエストモデル</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(ErrorObject))]
    [HttpPost]
    [Route("api/v{version:apiVersion}/external/dataImport/constantlyData")]
    public IActionResult StoreConstantlyDataAsync([FromBody] S3EventRequest s3EventRequest)
    {
        // DBとバケットの検証

        if (string.IsNullOrWhiteSpace(s3EventRequest.BucketName) || string.IsNullOrWhiteSpace(s3EventRequest.ObjectKey))
        {
            return BadRequest();
        }

        _ = _serviceScopeTaskRunner.Run<IDataImportUsecase>((dataImportUseCase) => dataImportUseCase.StoreConstantlyDataAsync(s3EventRequest.BucketName, s3EventRequest.ObjectKey));

        return Accepted();

    }

    /// <summary>
    /// EC1002_ファイル取り込みを実行する_日次
    /// </summary>
    /// <param name="s3EventRequest">S3イベントのリクエストモデル</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(ErrorObject))]
    [HttpPost]
    [Route("api/v{version:apiVersion}/external/dataImport/dailyData")]
    public IActionResult StoreDailyDataAsync([FromBody] S3EventRequest s3EventRequest)
    {
        if (string.IsNullOrWhiteSpace(s3EventRequest.BucketName))
        {
            return BadRequest();
        }

        _ = _serviceScopeTaskRunner.Run<IDataImportUsecase>((dataImportUseCase) => dataImportUseCase.StoreDailyDataAsync(s3EventRequest.BucketName));

        return Accepted();

    }
}
using Asp.Versioning;

using Microsoft.AspNetCore.Mvc;

using NSwag.Annotations;

using Ryobi.Wellship.APIModels.Requests;
using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Controllers.V1;
/// <summary>
/// ファイル取り込み コントローラ
/// </summary>
[ApiController]
[ApiVersion("1")]
[OpenApiIgnore]
public class DataImportController : ControllerBase
{
    private readonly IDataImportUsecase _dataImportUsecase;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="dataImportUsecase">ファイル取り込み ユースケース</param>
    public DataImportController(IDataImportUsecase dataImportUsecase)
    {
        _dataImportUsecase = dataImportUsecase;
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

        var task = Task.Run(() => _dataImportUsecase.StoreConstantlyDataAsync(s3EventRequest.BucketName, s3EventRequest.ObjectKey)); // 別スレッドで非同期メソッドを実行

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

        var task = Task.Run(() => _dataImportUsecase.StoreDailyDataAsync(s3EventRequest.BucketName)); // 別スレッドで非同期メソッドを実行

        return Accepted();

    }
}
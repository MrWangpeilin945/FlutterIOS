using Microsoft.AspNetCore.Mvc;

using Ryobi.Wellship.APIModels.Responses;
using Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Controllers.V1;

/// <summary>
/// 検査メニューコントローラー
/// </summary>
[ApiController]
[ApiVersion("1")]
public class ExamMenuController : ControllerBase
{
    private readonly IExamMenuUsecase _examMenuUsecase;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="examMenuUsecase">検査メニューユースケース</param>
    public ExamMenuController(IExamMenuUsecase examMenuUsecase)
    {
        _examMenuUsecase = examMenuUsecase;
    }

    /// <summary>
    /// 検査メニュー一覧を取得する
    /// </summary>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExamMenuList))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet]
    [Route("api/v{version:apiVersion}/examMenus")]
    public async Task<IActionResult> GetExamMenusAsync()
    {
        var results = await _examMenuUsecase.GetExamMenusAsync();
        return Ok(results);
    }
}

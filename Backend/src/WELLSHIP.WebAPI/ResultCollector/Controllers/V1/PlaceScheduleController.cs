using Microsoft.AspNetCore.Mvc;

using Ryobi.Wellship.APIModels.Responses;
using Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class PlaceScheduleController : ControllerBase
{
    private readonly IPlaceScheduleUsecase _placeScheduleUsecase;
    public PlaceScheduleController(IPlaceScheduleUsecase placeScheduleUsecase)
    {
        _placeScheduleUsecase = placeScheduleUsecase;
    }

    [HttpGet]
    public IEnumerable<PlaceSchedule> GetList([FromQuery] string? date, [FromQuery] int? teamId, [FromQuery] int? placeId)
    {
        var results = _placeScheduleUsecase.GetList(date, teamId, placeId);
        return results;
    }
}

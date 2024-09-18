using Microsoft.AspNetCore.Mvc;

using Ryobi.WELLSHIP.APIModels.Responses;
using Ryobi.WELLSHIP.WebAPI.ResultCollector.Usecases;

namespace Ryobi.WELLSHIP.WebAPI.ResultCollector.Controllers;

[ApiController]
[Route("[controller]")]
public class PlaceScheduleController : ControllerBase
{
    private readonly IPlaceScheduleUsecase _placeScheduleUsecase;
    public PlaceScheduleController(IPlaceScheduleUsecase placeScheduleUsecase)
    {
        _placeScheduleUsecase = placeScheduleUsecase;
    }

    [HttpGet]
    public IEnumerable<PlaceSchedule> GetList([FromQuery] string date, [FromQuery] int teamId, [FromQuery] int placeId)
    {
        var results = _placeScheduleUsecase.GetList(date, teamId, placeId);
        return results;
    }
}

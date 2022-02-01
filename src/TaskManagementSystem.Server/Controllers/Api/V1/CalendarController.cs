using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.BusinessLogic.Models;
using TaskManagementSystem.BusinessLogic.Services.Implementations;
using TaskManagementSystem.Shared.Models;
using CalendarEvent=TaskManagementSystem.Shared.Models.CalendarEvent;

namespace TaskManagementSystem.Server.Controllers.Api.V1;

[ApiController]
[Authorize]
[Route("Api/V1/[controller]")]
public class CalendarController : ControllerBase
{
    private readonly CalendarService calendarService;


    public CalendarController(CalendarService calendarService)
    {
        this.calendarService = calendarService;
    }

    [HttpPost("GetEventsForMonth")]
    public async Task<IActionResult> RegisterUser(CalendarGetEventsRequest request)
    {
        Result<IEnumerable<BusinessLogic.Models.CalendarEvent>> result;

        result = await calendarService.GetEvents2(request.StartTime.UtcDateTime, request.EndTime.UtcDateTime);

        if (result.IsSuccess)
        {
            var events = result.Value.Select(x => new CalendarEvent(x.Name, x.UtcStartTime, x.UtcEndTime)).ToArray();

            return Ok(new CalendarResponse(true, events, null));
        }

        return Ok(new CalendarResponse(false, null, result.ErrorDescription));
    }
}
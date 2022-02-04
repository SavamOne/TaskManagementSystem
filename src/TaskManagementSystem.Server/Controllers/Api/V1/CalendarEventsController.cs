using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.BusinessLogic.Services.Implementations;
using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Server.Controllers.Api.V1;

[ApiController]
[Authorize]
[Route("Api/V1/[controller]")]
public class CalendarEventsController : ControllerBase
{
    private readonly CalendarEventsService calendarEventsService;

    public CalendarEventsController(CalendarEventsService calendarEventsService)
    {
        this.calendarEventsService = calendarEventsService;
    }

    [HttpPost("GetEvents")]
    public async Task<IActionResult> GetEvents(CalendarGetEventsRequest request)
    {
        var result = await calendarEventsService.GetEvents(request.StartTime.UtcDateTime, request.EndTime.UtcDateTime);

        if (result.IsSuccess)
        {
            var events = result.Value!.Select(x => new CalendarEventInfo(x.Name, x.UtcStartTime, x.UtcEndTime)).ToArray();

            return Ok(new CalendarResponse(true, events, null));
        }

        return Ok(new CalendarResponse(false, null, result.ErrorDescription));
    }
}
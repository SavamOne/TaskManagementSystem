using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.BusinessLogic.Services.Implementations;
using TaskManagementSystem.Server.Filters;
using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Server.Controllers.Api.V1;

[ApiController]
[Authorize]
[ServiceFilter(typeof(ApiResponseExceptionFilter))]
[Route("Api/V1/[controller]")]
public class CalendarEventsController_Temp : ControllerBase
{
    private readonly Temp_CalendarEventService calendarEventServiceTemp;

    public CalendarEventsController_Temp(Temp_CalendarEventService calendarEventServiceTemp)
    {
        this.calendarEventServiceTemp = calendarEventServiceTemp;
    }

    [HttpPost("GetEvents")]
    public async Task<IActionResult> GetEvents(CalendarGetEventsRequest request)
    {
        var result = await calendarEventServiceTemp.GetEvents(request.StartTime.UtcDateTime, request.EndTime.UtcDateTime);

        var events = result.Select(x => new Temp_CalendarEventInfo(x.Name, x.UtcStartTime, x.UtcEndTime))
            .ToArray();

        return Ok(events);
    }
}
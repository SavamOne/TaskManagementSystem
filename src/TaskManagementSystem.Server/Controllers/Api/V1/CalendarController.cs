using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.BusinessLogic.Models.Models;
using TaskManagementSystem.BusinessLogic.Models.Requests;
using TaskManagementSystem.BusinessLogic.Services;
using TaskManagementSystem.BusinessLogic.Services.Implementations;
using TaskManagementSystem.Server.Filters;
using TaskManagementSystem.Server.Services;
using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Server.Controllers.Api.V1;

[Authorize]
[ApiController]
[ServiceFilter(typeof(ApiResponseExceptionFilter))]
[Route("Api/V1/[controller]")]
public class CalendarController : ControllerBase
{
    private readonly ITokenService tokenService;
    private readonly IUserService userService;
    private readonly CalendarService calendarService;
    
    public CalendarController(ITokenService tokenService, IUserService userService, CalendarService calendarService)
    {
        this.tokenService = tokenService;
        this.userService = userService;
        this.calendarService = calendarService;
    }
    
    [HttpPost("GetMyList")]
    public async Task<IActionResult> GetUserCalendarListAsync()
    {
        Guid userId = tokenService.GetUserIdFromClaims(User);

        var result = await calendarService.GetUserCalendars(userId);
        
        return Ok(result.Select(x => new CalendarInfo(x.Id, x.Name, x.Description, x.CreationDateUtc)));
    }

    [HttpPost("Create")]
    public async Task<IActionResult> CreateCalendarAsync(CreateCalendarRequest request)
    {
        Guid userId = tokenService.GetUserIdFromClaims(User);

        Calendar result = await calendarService.CreateCalendarAsync(new CreateCalendarData(userId, request.Name, request.Description));

        return Ok(new CalendarInfo(result.Id, result.Name, result.Description, result.CreationDateUtc));
    }
    
    [HttpPost("Edit")]
    public async Task<IActionResult> EditCalendarAsync(EditCalendarRequest request)
    {
        Guid userId = tokenService.GetUserIdFromClaims(User);
        
        Calendar result = await calendarService.EditCalendarAsync(new CalendarEditData(
        userId,  request.CalendarId, request.Name, request.Description));
        
        return Ok(new CalendarInfo(result.Id, result.Name, result.Description, result.CreationDateUtc));
    }
    
    [HttpPost("AddParticipants")]
    public async Task<IActionResult> AddParticipantsAsync(AddCalendarParticipantsRequest request)
    {
        Guid userId = tokenService.GetUserIdFromClaims(User);
        
        CalendarWithParticipants result = await calendarService.AddParticipantsAsync(new AddCalendarParticipantsData(
            userId, request.CalendarId, 
            request.Users.Select(x => new AddCalendarParticipantData(x.UserId, (CalendarRole)x.Role)).ToHashSet()
        ));
         
        return Ok(await ConvertAsync(result));
    }
    
    [HttpPost("ChangeParticipantsRole")]
    public async Task<IActionResult> ChangeParticipantsRoleAsync(ChangeCalendarParticipantsRoleRequest request)
    {
        Guid userId = tokenService.GetUserIdFromClaims(User);
        
        CalendarWithParticipants result = await calendarService.ChangeParticipantRoleAsync(new ChangeParticipantsRoleData(
        userId, request.CalendarId, 
        request.Participants.Select(x => new ChangeParticipantRoleData(x.ParticipantId, (CalendarRole)x.Role)).ToHashSet()
        ));
         
        return Ok(await ConvertAsync(result));
    }
    
    [HttpPost("DeleteParticipants")]
    public async Task<IActionResult> DeleteParticipantsAsync(DeleteParticipantsRequest request)
    {
        Guid userId = tokenService.GetUserIdFromClaims(User);
        
        CalendarWithParticipants result = await calendarService.DeleteParticipantsAsync(new DeleteParticipantsData(
        userId, request.CalendarId, request.ParticipantIds.ToHashSet()));
        
        return Ok(await ConvertAsync(result));
    }
    
    
    [HttpPost("GetInfo")]
    public async Task<IActionResult> GetCalendarInfoAsync(GetCalendarInfoRequest request)
    {
        CalendarWithParticipants result = await calendarService.GetCalendarInfoAsync(request.CalendarId);

        return Ok(await ConvertAsync(result));
    }

    private async Task<CalendarWithParticipantUsers> ConvertAsync(CalendarWithParticipants request)
    {
        var users = await userService.GetUsersAsync(request.Participants.Select(x => x.UserId).ToHashSet());
        var userDict = users.ToDictionary(x => x.Id);

        var participantsUsers = request.Participants.Select(participant =>
        {
            User user = userDict[participant.UserId];
            return new CalendarParticipantUser(participant.Id, participant.CalendarId, participant.UserId, participant.JoinDateUtc, 
                (CalendarParticipantRole)participant.Role, user.Name, user.Email, user.DateJoinedUtc);
        });
        
        return new CalendarWithParticipantUsers(
            new CalendarInfo(request.Calendar.Id, request.Calendar.Name, request.Calendar.Description, request.Calendar.CreationDateUtc),
            participantsUsers
        );
    }
    
}
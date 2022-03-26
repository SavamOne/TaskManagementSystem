using Microsoft.Extensions.DependencyInjection;
using TaskManagementSystem.BusinessLogic.Dal.Extensions;
using TaskManagementSystem.BusinessLogic.Services;
using TaskManagementSystem.BusinessLogic.Services.Implementations;

namespace TaskManagementSystem.BusinessLogic.Extensions;

public static class BusinessLogicRegistrationExtensions
{
    public static IServiceCollection AddBusinessLogic(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddBusinessLogicDal();
        serviceCollection.AddScoped<IUserService, UserService>();
        serviceCollection.AddScoped<ICalendarService, CalendarService>();
        serviceCollection.AddScoped<ICalendarEventService, CalendarEventService>();
        serviceCollection.AddScoped<Temp_CalendarEventService>();
        

        return serviceCollection;
    }
}
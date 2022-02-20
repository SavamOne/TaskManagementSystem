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
        serviceCollection.AddScoped<CalendarEventsService>();
        serviceCollection.AddScoped<CalendarService>();

        return serviceCollection;
    }
}
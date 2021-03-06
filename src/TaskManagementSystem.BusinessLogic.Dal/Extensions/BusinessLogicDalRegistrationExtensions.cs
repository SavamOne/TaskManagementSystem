using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TaskManagementSystem.BusinessLogic.Dal.Repositories;
using TaskManagementSystem.BusinessLogic.Dal.Repositories.Implementations;
using TaskManagementSystem.Shared.Dal.Extensions;

namespace TaskManagementSystem.BusinessLogic.Dal.Extensions;

public static class BusinessLogicDalRegistrationExtensions
{
	public static IServiceCollection AddBusinessLogicDal(this IServiceCollection serviceCollection)
	{
		serviceCollection.AddDal();

		serviceCollection.TryAddScoped<IUserRepository, UserRepository>();
		serviceCollection.TryAddScoped<ICalendarRepository, CalendarRepository>();
		serviceCollection.TryAddScoped<ICalendarParticipantRepository, CalendarParticipantRepository>();
		serviceCollection.TryAddScoped<ICalendarEventRepository, CalendarEventRepository>();
		serviceCollection.TryAddScoped<ICalendarEventParticipantRepository, CalendarEventParticipantRepository>();
		serviceCollection.TryAddScoped<IRecurrentSettingsRepository, RecurrentSettingsRepository>();

		return serviceCollection;
	}
}
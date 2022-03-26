using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace TaskManagementSystem.Shared.Dal.Extensions;

public static class SharedDalRegistrationExtensions
{
	public static IServiceCollection AddDal(this IServiceCollection serviceCollection)
	{
		DefaultTypeMap.MatchNamesWithUnderscores = true;

		serviceCollection.TryAddScoped<DatabaseConnectionProvider>();
		serviceCollection.TryAddScoped<IUnitOfWork, UnitOfWork>();

		return serviceCollection;
	}
}
using Microsoft.Extensions.DependencyInjection;
using TaskManagementSystem.Server.Dal.Repositories;
using TaskManagementSystem.Server.Dal.Repositories.Implementations;
using TaskManagementSystem.Shared.Dal.Extensions;

namespace TaskManagementSystem.Server.Dal.Extensions;

public static class ServerDalRegistrationExtensions
{
    public static IServiceCollection AddServerDal(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddDal();
        
        serviceCollection.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        
        return serviceCollection;
    }
    
    
}
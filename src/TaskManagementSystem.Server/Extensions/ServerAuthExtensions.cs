using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TaskManagementSystem.Server.Dal.Extensions;
using TaskManagementSystem.Server.Options;
using TaskManagementSystem.Server.Services;
using TaskManagementSystem.Server.Services.Implementations;
using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.Server.Extensions;

public static class ServerAuthExtensions
{
    public static IServiceCollection AddServerAuth(this IServiceCollection serviceCollection, IConfigurationSection jwtSection)
    {
        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

        serviceCollection.AddServerDal();
        serviceCollection.AddScoped<ITokenService, TokenService>();
        
        JwtOptions jwtOptions = ConfigureJwtOptions(serviceCollection, jwtSection);
        
        serviceCollection.AddAuthorization();
        serviceCollection.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtOptions.Audience,
                ValidateLifetime = true,
                IssuerSigningKey = jwtOptions.SymmetricAccessKey,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero
            };
        });
        
        return serviceCollection;
    }
    
    private static JwtOptions ConfigureJwtOptions(IServiceCollection serviceCollection, IConfigurationSection jwtSection)
    {
        jwtSection.AssertNotNull(nameof(jwtSection));

        JwtOptions jwtOptions = new();
        jwtSection.Bind(jwtOptions);

        jwtOptions.Audience.AssertNotNullOrWhiteSpace();
        jwtOptions.Issuer.AssertNotNullOrWhiteSpace();
        jwtOptions.AccessTokenSecretKey.AssertNotNullOrWhiteSpace();
        jwtOptions.RefreshTokenSecretKey.AssertNotNullOrWhiteSpace();

        serviceCollection.Configure<JwtOptions>(jwtSection);

        return jwtOptions;
    }
}
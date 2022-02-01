using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TaskManagementSystem.BusinessLogic.Services;
using TaskManagementSystem.BusinessLogic.Services.Implementations;
using TaskManagementSystem.Server.Options;
using TaskManagementSystem.Server.Services;
using TaskManagementSystem.Server.Services.Implementations;
using TaskManagementSystem.Shared.Helpers;
using TaskManagementSystem.Shared.Models.Options;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

JwtOptions jwtOptions = ConfigureJwtOptions(builder);
builder.Services.AddSingleton<CalendarService>();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<ITokenService, TokenService>();

builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Encoder = ApplicationJsonOptions.Encoder;
        options.JsonSerializerOptions.DefaultIgnoreCondition = ApplicationJsonOptions.DefaultIgnoreCondition;
        options.JsonSerializerOptions.AllowTrailingCommas = ApplicationJsonOptions.AllowTrailingCommas;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = ApplicationJsonOptions.PropertyNameCaseInsensitive;
        options.JsonSerializerOptions.PropertyNamingPolicy = ApplicationJsonOptions.PropertyNamingPolicy;
    });

builder.Services.AddRazorPages();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
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

builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
}

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseAuthentication();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

await app.RunAsync();

JwtOptions ConfigureJwtOptions(WebApplicationBuilder builder)
{
    IConfigurationSection? jwtSection = builder.Configuration.GetSection(nameof(JwtOptions));
    jwtSection.AssertNotNull(nameof(jwtSection));

    JwtOptions jwtOptions = new();
    jwtSection.Bind(jwtOptions);

    jwtOptions.Audience.AssertNotNullOrWhiteSpace();
    jwtOptions.Issuer.AssertNotNullOrWhiteSpace();
    jwtOptions.AccessTokenSecretKey.AssertNotNullOrWhiteSpace();
    jwtOptions.RefreshTokenSecretKey.AssertNotNullOrWhiteSpace();

    builder.Services.Configure<JwtOptions>(jwtSection);

    return jwtOptions;
}
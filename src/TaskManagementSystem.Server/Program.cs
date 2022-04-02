using Microsoft.AspNetCore.Localization;
using TaskManagementSystem.BusinessLogic.Extensions;
using TaskManagementSystem.Server.Extensions;
using TaskManagementSystem.Server.Filters;
using TaskManagementSystem.Server.Options;
using TaskManagementSystem.Shared.Dal.Options;
using TaskManagementSystem.Shared.Models.Options;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

//TODO: Придумать, как передавать секцию без установки дополнительного нугета.
builder.Services.Configure<PostgresOptions>(builder.Configuration.GetSection(nameof(PostgresOptions)));
builder.Services.AddBusinessLogic();
builder.Services.AddServerAuth(builder.Configuration.GetSection(nameof(JwtOptions)), builder.Configuration.GetSection(nameof(WebPushOptions)));

builder.Services.AddSingleton<ApiResponseExceptionFilter>();
builder.Services.AddControllersWithViews()
   .ConfigureApiBehaviorOptions(options =>
	{
		options.SuppressModelStateInvalidFilter = true;
		options.SuppressMapClientErrors = true;
	})
   .AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.Encoder = ApplicationJsonOptions.Encoder;
		options.JsonSerializerOptions.DefaultIgnoreCondition = ApplicationJsonOptions.DefaultIgnoreCondition;
		options.JsonSerializerOptions.AllowTrailingCommas = ApplicationJsonOptions.AllowTrailingCommas;
		options.JsonSerializerOptions.PropertyNameCaseInsensitive = ApplicationJsonOptions.PropertyNameCaseInsensitive;
		options.JsonSerializerOptions.PropertyNamingPolicy = ApplicationJsonOptions.PropertyNamingPolicy;
	});

builder.Services.AddRazorPages();

if (builder.Environment.IsDevelopment())
{
	builder.Services.AddSwaggerGen();
}

WebApplication app = builder.Build();


if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();

	app.UseWebAssemblyDebugging();
}

app.UseRequestLocalization(options =>
{
	options.DefaultRequestCulture = new RequestCulture(LocalizationOptions.DefaultCultureInfo);
	options.SupportedCultures = LocalizationOptions.AvailableCultureInfos;
	options.SupportedUICultures = LocalizationOptions.AvailableCultureInfos;
});

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseAuthentication();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

await app.RunAsync();
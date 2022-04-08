using Microsoft.AspNetCore.Localization;
using Microsoft.OpenApi.Models;
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
		options.JsonSerializerOptions.Converters.Add(ApplicationJsonOptions.Converters.First());
	});

builder.Services.AddRazorPages();

if (builder.Environment.IsDevelopment())
{
	builder.Services.AddSwaggerGen(options =>
	{
		options.SwaggerDoc("API", new OpenApiInfo
		{
			Version = "v1",
			Title = "TaskManagementSystem Public API Спецификация (v1)",
			Description = "Web API сервиса по работе с календарями и событиями",
			Contact = new OpenApiContact
			{
				Name = "GitHub",
				Url = new Uri("https://github.com/SavamOne/TaskManagementSystem")
			}
		});
		options.DescribeAllParametersInCamelCase();
		foreach (string xmlDocPath in Directory.GetFiles(AppContext.BaseDirectory, "*.xml"))
		{
			options.IncludeXmlComments(xmlDocPath);
		}
	});
}

WebApplication app = builder.Build();


if (app.Environment.IsDevelopment())
{
	app.UseSwagger(options =>
	{
		options.SerializeAsV2 = true;
	});
	app.UseSwaggerUI(options =>
	{
		options.SwaggerEndpoint("/swagger/API/swagger.yaml", "API");
	});

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
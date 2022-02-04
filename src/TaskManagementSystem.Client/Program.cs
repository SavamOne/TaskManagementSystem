using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TaskManagementSystem.Client;
using TaskManagementSystem.Client.Helpers;
using TaskManagementSystem.Client.Helpers.Implementations;
using TaskManagementSystem.Client.Providers;
using TaskManagementSystem.Client.Proxies;
using TaskManagementSystem.Client.Services;
using TaskManagementSystem.Client.Services.Implementations;

WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(_ => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});
builder.Services.AddScoped<ServerProxy>();

builder.Services.AddScoped<JwtAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>(provider =>
    provider.GetService<JwtAuthenticationStateProvider>()!);

builder.Services.AddScoped<ILocalStorageWrapper, LocalStorageWrapper>();
builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();

builder.Services.AddAuthorizationCore();

await builder.Build().RunAsync();
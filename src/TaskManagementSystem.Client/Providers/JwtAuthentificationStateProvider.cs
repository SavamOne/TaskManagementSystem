using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using TaskManagementSystem.Client.Services;
using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.Client.Providers;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private static readonly AuthenticationState AnonymousState = new(new ClaimsPrincipal());
    private static readonly AuthenticationState AuthorizedState = new(new ClaimsPrincipal(new ClaimsIdentity("Token")));

    private readonly ILocalTokensService storageService;

    public JwtAuthenticationStateProvider(ILocalTokensService storageService)
    {
        this.storageService = storageService.AssertNotNull();
    }

    private AuthenticationState CurrentState { get; set; } = AnonymousState;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        string token = await storageService.GetAccessTokenAsync();

        return CurrentState = !string.IsNullOrWhiteSpace(token) ? AuthorizedState : AnonymousState;
    }

    public void ChangeAuthenticationState(bool isAuthenticated)
    {
        CurrentState = isAuthenticated ? AuthorizedState : AnonymousState;
        NotifyAuthenticationStateChanged(Task.FromResult(CurrentState));
    }
}
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using TaskManagementSystem.Client.Services;

namespace TaskManagementSystem.Client.Providers;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private static readonly AuthenticationState AnonymousState = new(new ClaimsPrincipal());
    private static readonly AuthenticationState AuthorizedState = new(new ClaimsPrincipal(new ClaimsIdentity("Token")));
    
    private readonly ILocalStorageService storageService;

    private AuthenticationState CurrentState { get; set; } = AnonymousState;

    public JwtAuthenticationStateProvider(ILocalStorageService storageService)
    {
        this.storageService = storageService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        string token = await storageService.GetAccessTokenAsync();
        
        return CurrentState = !string.IsNullOrWhiteSpace(token) ? AuthorizedState : AnonymousState;
        //
        // if (string.IsNullOrWhiteSpace(token))
        // {
        //     return AnonymousState;
        // }
        // Console.WriteLine(token);
        // JwtSecurityToken? jwtSecurityToken = JwtSecurityTokenHandler.ReadJwtToken(token);
        //
        // if (jwtSecurityToken == null)
        // {
        //     return AnonymousState;
        // }
        //
        // DateTime expirationDate = jwtSecurityToken.Payload.ValidTo;
        // Console.WriteLine(expirationDate);

        // return Task.FromResult(CurrentState);
    }

    public void ChangeAuthenticationState(bool isAuthenticated)
    {
        CurrentState = isAuthenticated ? AuthorizedState : AnonymousState;
        NotifyAuthenticationStateChanged(Task.FromResult(CurrentState));
    }
}
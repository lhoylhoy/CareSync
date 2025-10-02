using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace CareSync.Web.Admin.Services;

public class LocalAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

    private readonly ClaimsPrincipal _localUser = new(new ClaimsIdentity(
        new[]
        {
            new Claim(ClaimTypes.Name, "Local Developer"), new Claim(ClaimTypes.Email, "developer@localhost.local"),
            new Claim(ClaimTypes.NameIdentifier, "local-dev-user-123"),
            new Claim("preferred_username", "developer@localhost.local")
        }, "local"));

    private bool _isAuthenticated = true; // For development, assume always authenticated

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = _isAuthenticated ? _localUser : _anonymous;
        return Task.FromResult(new AuthenticationState(user));
    }

    public void ToggleAuthentication()
    {
        _isAuthenticated = !_isAuthenticated;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public void SetAuthenticated(bool isAuthenticated)
    {
        _isAuthenticated = isAuthenticated;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}

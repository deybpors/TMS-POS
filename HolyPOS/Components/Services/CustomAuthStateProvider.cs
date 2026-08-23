using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace HolyPOS.Components.Services;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private const string StorageKey = "userSession";

    private ClaimsPrincipal _currentUser =
        new ClaimsPrincipal(new ClaimsIdentity());

    private readonly ProtectedLocalStorage _localStorage;

    public CustomAuthStateProvider(ProtectedLocalStorage localStorage)
    {
        _localStorage = localStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // If we already have a user loaded in memory (e.g. right after login),
        // no need to hit storage again.
        if (_currentUser.Identity?.IsAuthenticated == true)
        {
            return new AuthenticationState(_currentUser);
        }

        try
        {
            var result = await _localStorage.GetAsync<UserSession>(StorageKey);

            if (result.Success && result.Value is not null)
            {
                var identity = new ClaimsIdentity(
                    new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, result.Value.Username),
                        new Claim(ClaimTypes.Role, result.Value.Role)
                    },
                    "BlazorAuth"
                );

                _currentUser = new ClaimsPrincipal(identity);
            }
        }
        catch (InvalidOperationException)
        {
            // JS interop isn't available yet (e.g. during prerendering).
            // Fall back to the anonymous user for this render; it will
            // resolve correctly once the circuit is fully connected.
        }

        return new AuthenticationState(_currentUser);
    }

    public async Task Login(string username, string role)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role)
        };

        var identity = new ClaimsIdentity(
            claims,
            "BlazorAuth"
        );

        _currentUser = new ClaimsPrincipal(identity);

        await _localStorage.SetAsync(StorageKey, new UserSession(username, role));

        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(_currentUser))
        );
    }

    public async Task Logout()
    {
        _currentUser = new ClaimsPrincipal(new ClaimsIdentity());

        await _localStorage.DeleteAsync(StorageKey);

        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(_currentUser))
        );
    }

    private record UserSession(string Username, string Role);
}
using System.Security.Claims;
using HolyPOS.Components.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace HolyPOS.Components.Services;


public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private const string StorageKey = "userSession";
    private const string LastStoresKey = "lastSelectedStores";

    // Custom claim type for the store — there's no built-in
    // ClaimTypes entry for this, so we just use a plain string key.
    public const string StoreIdClaimType = "StoreId";

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
                _currentUser = BuildPrincipal(result.Value);
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

    public bool IsRole(string role)
    {
        return _currentUser.IsInRole(role);
    }

    public async Task Login(
        string username,
        string role,
        Guid? storeId = null)
    {
        // ============================================================
        // GET THIS USER'S LAST SELECTED STORE
        // ============================================================

        if (!storeId.HasValue)
        {
            try
            {
                var result =
                    await _localStorage.GetAsync<
                        Dictionary<string, Guid>>(
                        LastStoresKey);

                if (result.Success &&
                    result.Value is not null &&
                    result.Value.TryGetValue(
                        username,
                        out var lastStoreId))
                {
                    storeId = lastStoreId;
                }
            }
            catch (InvalidOperationException)
            {
                // JS interop unavailable.
            }
        }


        // ============================================================
        // CREATE SESSION
        // ============================================================

        var session =
            new UserSession(
                username,
                role,
                storeId);

        _currentUser =
            BuildPrincipal(session);


        // ============================================================
        // SAVE ACTIVE SESSION
        // ============================================================

        await _localStorage.SetAsync(
            StorageKey,
            session);


        NotifyAuthenticationStateChanged(
            Task.FromResult(
                new AuthenticationState(
                    _currentUser)));
    }

    // Lets a cashier pick/change their store after logging in, without
    // needing to log out and back in. Keeps username/role intact.
    public async Task SetStoreAsync(Guid storeId)
    {
        var username =
            _currentUser.Identity?.Name ?? "";

        var role =
            _currentUser.FindFirst(
                ClaimTypes.Role)?.Value ?? "";


        // ============================================================
        // GET EXISTING USER → STORE MAP
        // ============================================================

        Dictionary<string, Guid> lastStores;

        try
        {
            var result =
                await _localStorage.GetAsync<
                    Dictionary<string, Guid>>(
                    LastStoresKey);

            lastStores =
                result.Success &&
                result.Value is not null
                    ? result.Value
                    : new Dictionary<string, Guid>();
        }
        catch (InvalidOperationException)
        {
            lastStores =
                new Dictionary<string, Guid>();
        }


        // ============================================================
        // SAVE THIS USER'S STORE
        // ============================================================

        lastStores[username] = storeId;


        await _localStorage.SetAsync(
            LastStoresKey,
            lastStores);


        // ============================================================
        // UPDATE ACTIVE SESSION
        // ============================================================

        var session =
            new UserSession(
                username,
                role,
                storeId);

        _currentUser =
            BuildPrincipal(session);


        await _localStorage.SetAsync(
            StorageKey,
            session);


        NotifyAuthenticationStateChanged(
            Task.FromResult(
                new AuthenticationState(
                    _currentUser)));
    }

    public async Task Logout()
    {
        _currentUser =
            new ClaimsPrincipal(
                new ClaimsIdentity());

        // ONLY remove the active session.
        //
        // Do NOT remove LastStoresKey.
        await _localStorage.DeleteAsync(
            StorageKey);

        NotifyAuthenticationStateChanged(
            Task.FromResult(
                new AuthenticationState(
                    _currentUser)));
    }


    // Returns the current user's stored StoreId claim, or the first store
// in the given list if none is set (e.g. a fresh login with no store chosen yet).
    public Guid? GetStoreId(IEnumerable<Store> stores)
    {
        var value = _currentUser.FindFirst(StoreIdClaimType)?.Value;

        if (Guid.TryParse(value, out var storeId))
        {
            return storeId;
        }

        return stores.FirstOrDefault()?.Id;
    }
    
    // Same as GetStoreId, but resolves the actual Store object.
    public Store GetStore(IEnumerable<Store> stores)
    {
        var storeId = GetStoreId(stores);
        return stores.FirstOrDefault(x => x.Id == storeId);
    }
    
    

    private static ClaimsPrincipal BuildPrincipal(UserSession session)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, session.Username),
            new Claim(ClaimTypes.Role, session.Role)
        };

        if (session.StoreId.HasValue)
        {
            claims.Add(
                new Claim(StoreIdClaimType, session.StoreId.Value.ToString()));
        }

        var identity = new ClaimsIdentity(claims, "BlazorAuth");

        return new ClaimsPrincipal(identity);
    }

    private record UserSession(string Username, string Role, Guid? StoreId);
}
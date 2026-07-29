using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using OzBiPortalCRM.Models;

namespace OzBiPortalCRM.Services
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ProtectedLocalStorage _protectedLocalStorage;
        private readonly ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        private UserSession? _cachedSession;

        public CustomAuthenticationStateProvider(ProtectedLocalStorage protectedLocalStorage)
        {
            _protectedLocalStorage = protectedLocalStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                if (_cachedSession != null)
                {
                    return CreateAuthState(_cachedSession);
                }

                var userSessionResult = await _protectedLocalStorage.GetAsync<UserSession>("UserSession");
                if (userSessionResult.Success && userSessionResult.Value != null)
                {
                    _cachedSession = userSessionResult.Value;
                    return CreateAuthState(_cachedSession);
                }
            }
            catch (InvalidOperationException)
            {
                // Prerendering phase: JS Interop is not available yet
                return new AuthenticationState(_anonymous);
            }
            catch (Exception)
            {
                return new AuthenticationState(_anonymous);
            }

            return new AuthenticationState(_anonymous);
        }

        public async Task MarkUserAsAuthenticatedAsync(PortalUser user)
        {
            var userSession = new UserSession
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role
            };

            _cachedSession = userSession;

            try
            {
                await _protectedLocalStorage.SetAsync("UserSession", userSession);
            }
            catch
            {
                // Ignore storage error if during prerender
            }

            var authState = CreateAuthState(userSession);
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
        }

        public async Task MarkUserAsLoggedOutAsync()
        {
            _cachedSession = null;
            try
            {
                await _protectedLocalStorage.DeleteAsync("UserSession");
            }
            catch
            {
                // Ignore storage error if during prerender
            }

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
        }

        private AuthenticationState CreateAuthState(UserSession session)
        {
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, session.Id.ToString()),
                new Claim(ClaimTypes.Name, session.FullName),
                new Claim(ClaimTypes.Email, session.Email),
                new Claim(ClaimTypes.Role, session.Role)
            }, "CustomAuth"));

            return new AuthenticationState(claimsPrincipal);
        }
    }

    public class UserSession
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}

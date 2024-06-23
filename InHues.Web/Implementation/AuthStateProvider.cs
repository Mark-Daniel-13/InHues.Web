using InHues.StateMngmt;
using InHues.StateMngmt.Storage;
using Microsoft.AspNetCore.Components;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace InHues.Web.Implementation
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly AuthenticationState _anonymous;
        private readonly IStorageMngmt _storageMngmt;
        private readonly AppKeys _appKeys;
        readonly NavigationManager NavManager;
        readonly AppState _appState;
        public AuthStateProvider(IStorageMngmt storageMngmt, AppKeys appKeys, NavigationManager navManager, AppState appState)
        {
            _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())); ;
            _storageMngmt = storageMngmt;
            _appKeys = appKeys;
            NavManager = navManager;
            _appState = appState;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var storage_token = await _storageMngmt?.GetValueAsync(_appKeys.AccessToken);
            if (string.IsNullOrEmpty(storage_token)) return _anonymous;

            if (IsTokenExired(storage_token)) {
                await _storageMngmt.FlushValues();
                _appState.Destroy();
                NavManager.NavigateTo("/", forceLoad: true);
            }

            var identity = new ClaimsIdentity(ParseClaimsFromJwt(storage_token), "jwt");
            var user = new ClaimsPrincipal(identity);
            var auth = new AuthenticationState(user);
            return auth;
        }
        bool IsTokenExired(string token)
        {
            // Perform the necessary logic to check the token expiration
            // You can use a library like System.IdentityModel.Tokens.Jwt to decode and validate the token
            // Here's an example of how you can check the expiration using the JwtSecurityToken class:
            var jwtToken = new JwtSecurityToken(token);
            return jwtToken.ValidTo < DateTime.UtcNow;
        }
        private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }

        public void NotifyUserLogIn(string token) {
            var authUser = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt"));
            var authState = Task.FromResult(new AuthenticationState(authUser));
            NotifyAuthenticationStateChanged(authState);
        }
        public void NotifyuserLogOut() {
            var authState = Task.FromResult(_anonymous);
            NotifyAuthenticationStateChanged(authState);
        }
    }
}

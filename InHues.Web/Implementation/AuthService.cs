using InHues.Components.ViewModels.Auth;
using InHues.Domain.DTO.V1.Identity.Request;
using InHues.Domain.DTO.V1.Identity.Response;
using InHues.Domain.Helpers;
using InHues.StateMngmt;
using InHues.StateMngmt.Storage;
using Radzen;

namespace InHues.Web.Implementation
{
    public class AuthService : IAuthService
    {
        readonly IBridge? _bridge;
        readonly NotificationService? _notifService;
        readonly IStorageMngmt _storageMngmt;
        readonly AuthenticationStateProvider _authStateProvider;
        readonly AppKeys _appkeys;
        readonly AppState _appState;
        public AuthService(IBridge? bridge, NotificationService? notifService, AuthenticationStateProvider authStateProvider, IStorageMngmt storageMngmt, AppKeys appKeys, AppState appState)
        {
            _bridge = bridge;
            _notifService = notifService;
            _authStateProvider = authStateProvider;
            _storageMngmt = storageMngmt;
            _appkeys = appKeys;
            _appState = appState;
        }
        public async Task LogoutAsync()
        {
            if (_storageMngmt is null) return;
            await _storageMngmt.FlushValues();
            _appState.Destroy();
            ((AuthStateProvider)_authStateProvider)?.NotifyuserLogOut();
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            if (_bridge is null) return false;

            var loginUser = await _bridge.CommandJsonAsync<AuthSuccessResponse, Login>(ApiRoutes.Identity.Login, new() { Email = username, Password = password}, RequestType.POST);
            if (!loginUser.IsSuccess || loginUser.Response?.Token is null)
            {

                var alert = new NotificationMessage
                {
                    Severity = NotificationSeverity.Warning,
                    Summary = "Invalid username and password,",
                    Detail = "Please try again.",
                    Duration = 1500
                };

                _notifService?.Notify(alert);
                return false;
            }


            await _storageMngmt.SetValueAsync(_appkeys.AccessToken,loginUser.Response.Token);
            await _storageMngmt.SetValueAsync(_appkeys.RefreshToken, loginUser.Response.RefreshToken);

            ((AuthStateProvider)_authStateProvider)?.NotifyUserLogIn(loginUser.Response.Token);
            return true;
        }

        public async Task<bool> RegisterAsync(RegisterIdentityRequest payload) {
            if (_bridge is null) return false;

            payload.UserName = payload.Email;
            var createQuery = await _bridge.CommandJsonAsync<AuthSuccessResponse, RegisterIdentityRequest>(ApiRoutes.Identity.Register, payload, RequestType.POST);
            if (!createQuery.IsSuccess)
            {
                if (createQuery.Errors.Any())
                {
                    createQuery.Errors.ToList().ForEach(err =>
                    {
                        _notifService?.Notify(new()
                        {

                            Severity = NotificationSeverity.Error,
                            Summary = err,
                            Duration = 1500
                        });
                    });
                }
                return false;
            }

            _notifService?.Notify(new()
            {

                Severity = NotificationSeverity.Success,
                Summary = "Account Successfully Created",
                Detail = "You can now log in using the credentials you created",
                Duration = 1500
            });


            await _storageMngmt.SetValueAsync(_appkeys.AccessToken, createQuery.Response.Token);
            await _storageMngmt.SetValueAsync(_appkeys.RefreshToken, createQuery.Response.RefreshToken);

            ((AuthStateProvider)_authStateProvider)?.NotifyUserLogIn(createQuery.Response.Token);
            return true;
        }
    }
}

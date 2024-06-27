using Blazored.LocalStorage;
using InHues.StateMngmt.Storage;

namespace InHues.StateMngmt
{
    public class AppState
    {
        ILocalStorageService _localStorageService;

        public AppState(ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }
        public bool IsNewInstance { get; private set; } = true;


        private string _selectedSidebarItem = string.Empty;
        private string _prevURL = string.Empty;
        private bool _isRunningRequest = false;
        UserViewModel? _currentUser;

        public string SelectedSidebarItem { get => _selectedSidebarItem; }
        public string PrevURL { get => _prevURL; }
        public bool IsRunningRequest { get => _isRunningRequest; }
        public UserViewModel? CurrentUser { get => _currentUser; }


        #region Setters
            public async Task RestoryStateAsync() {
                if (!IsNewInstance) return;

                var storageData = await _localStorageService.GetItemAsync<AppStateSkeleton>("cst");

                if (storageData is not null)
                {
                    _selectedSidebarItem = storageData.SelectedSidebarItem;
                    _isRunningRequest = storageData.IsRunningRequest;
                    _currentUser = storageData.CurrentUser;
                }
                IsNewInstance = false;
                Console.WriteLine("State Restored");
            }
            public void SyncStorage() {
                _localStorageService.SetItemAsync("cst", this);
            }
            public void SetSidebarItem (string value) {
                _selectedSidebarItem = value;
            }
            public void SetIsRunningRequest(bool value)
            {
                _isRunningRequest = value;
            }
            public void SetPrevURL(string value)
            {
                _prevURL = value;
            }
            public void SetCurrentUser(UserViewModel value)
            {
                _currentUser = value;
            }
        #endregion

        public void Destroy() {
            _selectedSidebarItem = string.Empty;
            _isRunningRequest = false;
            _currentUser = null;
            _prevURL = string.Empty;
        }
    }
}

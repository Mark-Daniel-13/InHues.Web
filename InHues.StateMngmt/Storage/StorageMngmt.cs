using Blazored.LocalStorage;

namespace InHues.StateMngmt.Storage
{
    public class StorageMngmt : IStorageMngmt
    {
        ILocalStorageService _localStorageService;
        public StorageMngmt(ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }

        public async Task SetValueAsync(string key,string value)
        {
            if (_localStorageService is null) return;
            await _localStorageService.SetItemAsync(key, value);
        }
        public async Task<string> GetValueAsync(string key)
        {
            if (_localStorageService is null) return string.Empty;
            return await _localStorageService.GetItemAsync<string>(key);
        }
        public async Task FlushValues() {
            await _localStorageService.ClearAsync();
        }
    }
}

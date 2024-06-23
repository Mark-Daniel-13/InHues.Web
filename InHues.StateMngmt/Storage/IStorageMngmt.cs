namespace InHues.StateMngmt.Storage
{
    public interface IStorageMngmt
    {
        public Task SetValueAsync(string key, string value);
        public Task<string> GetValueAsync(string key);
        public Task FlushValues();
    }
}

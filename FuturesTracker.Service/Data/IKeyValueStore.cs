namespace FuturesTracker.Service.Data
{
    public interface IKeyValueStore
    {
        T? Get<T>(string key) where T : class;

        void Put<T>(string key, T? value) where T : class;

        void Remove(string key);
    }
}

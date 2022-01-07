using RocksDbSharp;
using System.Text.Json;

namespace FuturesTracker.Service.Data
{
    public class RocksDbStore : IKeyValueStore, IDisposable
    {
        private RocksDb db;

        public RocksDbStore(IConfiguration config)
        {
            var options = new DbOptions().SetCreateIfMissing(true);
            var path = Path.Join(Environment.CurrentDirectory, config.GetValue("DataStoreLocation", "tickerdata"));
            db = RocksDb.Open(options, path);
        }

        public T? Get<T>(string key) where T : class
        {
            var result = db?.Get(key);
            if (result == null) return null;
            return JsonSerializer.Deserialize<T>(db.Get(key));
        }

        public void Put<T>(string key, T? value) where T : class
        {
            if (value == null) throw new ArgumentNullException("value");
            var result = JsonSerializer.Serialize(value);
            db?.Put(key, result);
        }

        public void Remove(string key)
        {
            db?.Remove(key);
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}

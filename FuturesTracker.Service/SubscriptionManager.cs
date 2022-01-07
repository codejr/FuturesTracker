using FuturesTracker.Service.Data;
using FuturesTracker.Service.Models;

namespace FuturesTracker.Service
{
    public class TickerTracker : ITickerTracker
    {
        record class TickerSubscription(string Ticker, bool IsInitialized, IDisposable Subscription);

        private readonly IKeyValueStore db;

        private readonly IDeribitClient client;

        private IDictionary<string, TickerSubscription> subscriptions;

        public TickerTracker(IKeyValueStore db, IDeribitClient client)
        {
            this.db = db;
            this.client = client;
            this.subscriptions = new Dictionary<string, TickerSubscription>();
        }

        public async Task<SubscriptionResponse?> GetTickerDataAsync(string ticker)
        {
            if (!subscriptions.ContainsKey(ticker))
            {
                var sub = await client.SubscribeAsync(ticker, OnResponse);
                subscriptions.Add(ticker, new TickerSubscription(ticker, false, sub));
            }

            return await GetWithTimeout(ticker);
        }

        private async Task<SubscriptionResponse?> GetWithTimeout(string ticker)
        {
            var sub = subscriptions[ticker];
            if (!sub.IsInitialized)
            {
                var autoEvent = new AutoResetEvent(false);
                var timeout = TimeSpan.FromSeconds(5);
                while (timeout.Ticks > 0)
                {
                    if (subscriptions[ticker].IsInitialized) break;
                    var delay = TimeSpan.FromMilliseconds(100);
                    timeout = timeout - delay;
                    await Task.Delay(delay);
                }
            }

            return db.Get<SubscriptionResponse>(ticker);
        }

        private void OnResponse(SubscriptionResponse resp)
        {
            if (!subscriptions.ContainsKey(resp.InstrumentName)) return;

            var sub = subscriptions[resp.InstrumentName];

            db.Put(sub.Ticker, resp);

            subscriptions[resp.InstrumentName] = sub with { IsInitialized = true };
        }
    }
}

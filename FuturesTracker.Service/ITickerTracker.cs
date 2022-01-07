using FuturesTracker.Service.Models;

namespace FuturesTracker.Service
{
    public interface ITickerTracker
    {
        Task<SubscriptionResponse?> GetTickerDataAsync(string ticker);
    }
}
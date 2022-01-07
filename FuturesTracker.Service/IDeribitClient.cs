using FuturesTracker.Service.Models;

namespace FuturesTracker.Service
{
    public interface IDeribitClient : IDisposable
    {
        Task<IDisposable> SubscribeAsync(string ticker, Action<SubscriptionResponse> subAction);
    }
}
using FuturesTracker.Service.Models;
using System.Text.Json;
using Websocket.Client;

namespace FuturesTracker.Service
{
    public class DeribitClient : IDisposable, IDeribitClient
    {
        private const string subscriptionMethod = "public/subscribe";

        private ISet<string> instruments;

        private WebsocketClient client;

        private Uri baseUri;

        public DeribitClient(IConfiguration config)
        {
            this.baseUri = new Uri(config.GetValue<string>("DeribitHost"));
            this.instruments = new HashSet<string>();
            this.client = new WebsocketClient(this.baseUri);
        }

        public async Task<IDisposable> SubscribeAsync(string ticker, Action<SubscriptionResponse> subAction)
        {
            if (!client.IsStarted) await client.Start();

            instruments.Add(ticker);

            var req = new
            {
                jsonrpc = "2.0",
                method = subscriptionMethod,
                @params = new
                {
                    channels = instruments.Select(m => $"ticker.{m}.raw").ToArray()
                }
            };

            var sub = client.MessageReceived.Subscribe(msg =>
            {
                var response = JsonSerializer.Deserialize<JsonRpcMessage<SubscriptionResponse>>(msg.Text, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

                if (response?.Method == "subscription"
                    && response?.Parameters?.Data?.InstrumentName == ticker)
                {
                    subAction.Invoke(response.Parameters.Data);
                }
            });

            await client.SendInstant(JsonSerializer.Serialize(req));

            return sub;
        }

        public void Dispose()
        {
            client.Dispose();
        }
    }
}

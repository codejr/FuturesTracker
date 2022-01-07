using System.Text.Json.Serialization;

namespace FuturesTracker.Service.Models
{
    public record class SubscriptionResponse(
        [property:JsonPropertyName("instrument_name")]
        string InstrumentName,

        [property:JsonPropertyName("max_price")]
        decimal MaxPrice,

        [property:JsonPropertyName("min_price")]
        decimal MinPrice,

        [property:JsonPropertyName("mark_price")]
        decimal MarkPrice,

        [property:JsonPropertyName("last_price")]
        decimal LastPrice);
}

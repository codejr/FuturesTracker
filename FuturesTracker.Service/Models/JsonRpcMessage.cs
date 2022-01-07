using System.Text.Json.Serialization;

namespace FuturesTracker.Service.Models
{
    public record class Parameters<T>(
        [property:JsonPropertyName("channel")]
        string Channel,

        [property:JsonPropertyName("data")]
        T Data);

    public record class JsonRpcMessage<T>(
        [property:JsonPropertyName("jsonrpc")]
        string Version,

        [property:JsonPropertyName("method")]
        string Method,

        [property:JsonPropertyName("params")]
        Parameters<T> Parameters
    );
}

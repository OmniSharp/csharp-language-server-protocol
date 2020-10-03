using Newtonsoft.Json;

namespace OmniSharp.Extensions.JsonRpc.Client
{
    public class OutgoingRequest
    {
        public object? Id { get; set; }

        public string? Method { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object? Params { get; set; }
    }
}

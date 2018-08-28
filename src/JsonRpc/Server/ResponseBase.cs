using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.JsonRpc.Server
{
    public class ResponseBase
    {
        public ResponseBase(object id)
        {
            Id = id;
        }

        [JsonProperty("jsonrpc")]
        public string ProtocolVersion { get; set; } = "2.0";

        public object Id { get; set; }
    }
}

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc.Server.Messages;

namespace OmniSharp.Extensions.JsonRpc.Server
{
    public class Response
    {
        public Response(object id, JToken result)
        {
            Id = id;
            Result = result;
        }

        public Response(object id, IErrorMessage error)
        {
            Id = id;
            Error = error;
        }

        public string ProtocolVersion { get; set; } = "2.0";

        public object Id { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public JToken Result { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IErrorMessage Error { get; set; }
    }
}

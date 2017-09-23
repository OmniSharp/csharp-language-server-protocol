using Newtonsoft.Json.Linq;

namespace OmniSharp.Extensions.JsonRpc.Server
{
    public class Response
    {
        public Response(object id, JToken result)
        {
            Id = id;
            Result = result;
        }

        public Response(object id, string error)
        {
            Id = id;
            Error = error;
        }

        public string ProtocolVersion { get; set; } = "2.0";

        public object Id { get; set; }

        public JToken Result { get; set; }

        public string Error { get; set; }
    }
}
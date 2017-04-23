using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace JsonRpc.Server
{
    public class Response
    {
        public Response(object id) : this(id, null)
        {
        }

        public Response(object id, JToken result)
        {
            Id = id;
            Result = result;
        }

        public string ProtocolVersion { get; set; } = "2.0";

        public object Id { get; set; }

        public JToken Result { get; set; }
    }
}
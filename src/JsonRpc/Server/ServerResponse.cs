using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc.Server.Messages;

namespace OmniSharp.Extensions.JsonRpc.Server
{
    public class ResponseBase
    {
        public ResponseBase(object id)
        {
            Id = id;
        }

        public string ProtocolVersion { get; set; } = "2.0";

        public object Id { get; set; }
    }

    public class ServerResponse : ResponseBase
    {
        public ServerResponse(object id, JToken result) : base(id)
        {
            Result = result;
        }

        public JToken Result { get; set; }
    }

    public class ServerError : ResponseBase
    {
        public ServerError(object id, JToken result) : base(id)
        {
            Error = result;
        }

        public JToken Error { get; set; }
    }
}

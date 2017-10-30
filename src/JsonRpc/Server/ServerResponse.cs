using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc.Server.Messages;

namespace OmniSharp.Extensions.JsonRpc.Server
{
    public class ServerResponse : ResponseBase
    {
        public ServerResponse(object id, JToken result) : base(id)
        {
            Result = result;
        }

        public JToken Result { get; set; }
    }
}

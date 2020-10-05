using Newtonsoft.Json.Linq;

namespace OmniSharp.Extensions.JsonRpc.Server
{
    public class ServerResponse : ResponseBase
    {
        public ServerResponse(object id, JToken? result) : base(id) => Result = result ?? JValue.CreateNull();

        public JToken Result { get; set; }
    }
}

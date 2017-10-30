using Newtonsoft.Json.Linq;

namespace OmniSharp.Extensions.JsonRpc.Server
{
    public class ServerError : ResponseBase
    {
        public ServerError(object id, JToken result) : base(id)
        {
            Error = result;
        }

        public JToken Error { get; set; }
    }
}
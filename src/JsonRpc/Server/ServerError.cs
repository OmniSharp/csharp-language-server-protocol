using System.Text.Json;

namespace OmniSharp.Extensions.JsonRpc.Server
{
    public class ServerError : ResponseBase
    {
        public ServerError(object id, JsonElement result) : base(id)
        {
            Error = result;
        }

        public JsonElement Error { get; set; }
    }
}

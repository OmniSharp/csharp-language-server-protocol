using System.Text.Json;

namespace OmniSharp.Extensions.JsonRpc.Server
{
    public class ServerResponse : ResponseBase
    {
        public ServerResponse(object id, JsonElement result) : base(id)
        {
            Result = result;
        }

        public JsonElement Result { get; set; }
    }
}

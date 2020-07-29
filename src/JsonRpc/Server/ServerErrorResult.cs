using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OmniSharp.Extensions.JsonRpc.Server
{
    public class ServerErrorResult
    {
        [JsonConstructor]
        public ServerErrorResult(int code, string message, JToken data)
        {
            Code = code;
            Message = message;
            Data = data;
        }
        public ServerErrorResult(int code, string message)
        {
            Code = code;
            Message = message;
            Data = new JObject();
        }

        public int Code { get; set; }
        public string Message { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public JToken Data { get; set; }
    }
}

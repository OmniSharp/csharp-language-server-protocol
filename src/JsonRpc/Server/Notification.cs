using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.JsonRpc.Server
{
    public class Notification : IMethodWithParams
    {
        internal Notification(
            string method,
            JToken @params)
        {
            Method = method;
            Params = @params;
        }

        public string Method { get; }

        public JToken Params { get; }
    }
}

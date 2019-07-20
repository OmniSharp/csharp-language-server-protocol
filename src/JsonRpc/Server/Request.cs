using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.JsonRpc.Server
{
    public class Request : IMethodWithParams
    {
        internal Request(
            object id,
            string method,
            JToken @params)
        {
            Id = id;
            Method = method;
            Params = @params;
        }

        public object Id { get; }

        public string Method { get; }

        public JToken Params { get; }
    }
}

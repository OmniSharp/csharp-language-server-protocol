using System.Text.Json;

namespace OmniSharp.Extensions.JsonRpc.Server
{
    public class Request : IMethodWithParams
    {
        internal Request(
            object id,
            string method,
            JsonElement @params)
        {
            Id = id;
            Method = method;
            Params = @params;
        }

        public object Id { get; }

        public string Method { get; }

        public JsonElement? Params { get; }
    }
}

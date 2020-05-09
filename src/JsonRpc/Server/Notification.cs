using System.Text.Json;

namespace OmniSharp.Extensions.JsonRpc.Server
{
    public class Notification : IMethodWithParams
    {
        internal Notification(
            string method,
            JsonElement @params)
        {
            Method = method;
            Params = @params;
        }

        public string Method { get; }

        public JsonElement Params { get; }
    }
}

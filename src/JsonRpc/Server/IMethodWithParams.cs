using Newtonsoft.Json.Linq;

namespace OmniSharp.Extensions.JsonRpc.Server
{
    public interface IMethodWithParams
    {
        string Method { get; }
        JToken Params { get; }
    }
}

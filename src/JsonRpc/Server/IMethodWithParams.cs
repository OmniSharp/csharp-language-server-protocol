using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.JsonRpc.Server
{
    public interface IMethodWithParams
    {
        string Method { get; }
        JToken Params { get; }
    }
}

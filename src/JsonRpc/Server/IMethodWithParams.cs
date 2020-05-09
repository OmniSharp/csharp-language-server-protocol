using System.Text.Json;

namespace OmniSharp.Extensions.JsonRpc.Server
{
    public interface IMethodWithParams
    {
        string Method { get; }
        JsonElement Params { get; }
    }
}

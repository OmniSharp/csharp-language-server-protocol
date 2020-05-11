using System;
using System.Text.Json;

namespace OmniSharp.Extensions.JsonRpc.Server
{
    public interface IMethodWithParams
    {
        string Method { get; }
        object Params { get; }
    }
}

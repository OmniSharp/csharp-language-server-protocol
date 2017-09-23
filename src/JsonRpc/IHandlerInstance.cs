using System;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IHandlerInstance
    {
        string Method { get; }
        IJsonRpcHandler Handler { get; }
        Type HandlerType { get; }
        Type Params { get; }
    }
}
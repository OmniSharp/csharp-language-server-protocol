using System;

namespace JsonRpc
{
    public interface IHandlerInstance
    {
        string Method { get; }
        IJsonRpcHandler Handler { get; }
        Type HandlerInterface { get; }
        Type Params { get; }
    }
}
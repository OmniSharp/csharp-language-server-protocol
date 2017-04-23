using System;

namespace JsonRpc
{
    public interface IHandlerInstance
    {
        string Method { get; }
        IJsonRpcHandler Handler { get; }
        Type HandlerType { get; }
        Type Params { get; }
    }
}
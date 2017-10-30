using System;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IHandlerDescriptor
    {
        string Method { get; }
        IJsonRpcHandler Handler { get; }
        Type HandlerType { get; }
        Type Params { get; }
    }
}

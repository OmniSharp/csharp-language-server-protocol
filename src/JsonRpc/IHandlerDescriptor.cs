using System;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IHandlerDescriptor
    {
        string Method { get; }
        Type HandlerType { get; }
        Type ImplementationType { get; }
        Type Params { get; }
        Type Response { get; }
        IJsonRpcHandler Handler { get; }
    }
}

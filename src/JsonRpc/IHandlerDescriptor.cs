using System;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IHandlerDescriptor
    {
        string Method { get; }
        Type HandlerType { get; }
        Type Params { get; }
        Type Response { get; }
    }

    public interface IHandlerDelegateDescriptor
    {
        Delegate HandlerDelegate { get; }
    }

    public interface IHandlerTypeDescriptor
    {
        Type HandlerImplemenationType { get; }
    }

    public interface IHandlerInstanceDescriptor
    {
        IJsonRpcHandler Handler { get; }
    }
}

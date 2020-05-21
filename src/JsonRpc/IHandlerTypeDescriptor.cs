using System;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IHandlerTypeDescriptor
    {
        string Method { get; }
        Direction Direction { get; }
        RequestProcessType? RequestProcessType { get; }
        Type InterfaceType { get; }
        bool IsNotification { get; }
        bool IsRequest { get; }
        Type HandlerType { get; }
        bool HasParamsType { get; }
        Type ParamsType { get; }
        bool HasResponseType { get; }
        Type ResponseType { get; }
    }
}

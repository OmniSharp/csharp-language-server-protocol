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

    /// <summary>
    /// Marker interface used to pick up eventing handlers and throw them into the container
    /// THis is so that consumers do not need to manually add two services to the service collection
    /// We'll let our internal container pick it up.
    /// </summary>
    public interface IEventingHandler
    {
    }
}

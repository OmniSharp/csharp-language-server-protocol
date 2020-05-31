using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using MediatR;

namespace OmniSharp.Extensions.JsonRpc
{
    [DebuggerDisplay("{" + nameof(Method) + "}")]
    class HandlerTypeDescriptor : IHandlerTypeDescriptor
    {
        public HandlerTypeDescriptor(Type handlerType)
        {
            var method = handlerType.GetCustomAttribute<MethodAttribute>();
            Method = method.Method;
            Direction = method.Direction;
            HandlerType = handlerType;
            InterfaceType = HandlerTypeDescriptorHelper.GetHandlerInterface(handlerType);

            ParamsType = InterfaceType.IsGenericType ? InterfaceType.GetGenericArguments()[0] : typeof(EmptyRequest);
            HasParamsType = ParamsType != null && ParamsType != typeof(EmptyRequest);

            IsNotification = typeof(IJsonRpcNotificationHandler).IsAssignableFrom(handlerType) || handlerType
                .GetInterfaces().Any(z =>
                    z.IsGenericType && typeof(IJsonRpcNotificationHandler<>).IsAssignableFrom(z.GetGenericTypeDefinition()));
            IsRequest = !IsNotification;

            var requestInterface = ParamsType?
                .GetInterfaces()
                .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRequest<>));
            if (requestInterface != null)
                ResponseType = requestInterface.GetGenericArguments()[0];
            HasResponseType = ResponseType != null && ResponseType != typeof(Unit);

            var processAttributes = HandlerType
                .GetCustomAttributes(true)
                .Concat(HandlerType.GetCustomAttributes(true))
                .Concat(InterfaceType.GetInterfaces().SelectMany(x => x.GetCustomAttributes(true)))
                .Concat(HandlerType.GetInterfaces().SelectMany(x => x.GetCustomAttributes(true)))
                .OfType<ProcessAttribute>()
                .ToArray();
            RequestProcessType = processAttributes
                .FirstOrDefault()?.Type;
        }

        public string Method { get; }
        public Direction Direction { get; }
        public RequestProcessType? RequestProcessType { get; }
        public bool IsRequest { get; }
        public Type HandlerType { get; }
        public Type InterfaceType { get; }
        public bool IsNotification { get; }
        public bool HasParamsType { get; }
        public Type ParamsType { get; }
        public bool HasResponseType { get; }
        public Type ResponseType { get; }
        public override string ToString() => $"{Method}";
    }
}

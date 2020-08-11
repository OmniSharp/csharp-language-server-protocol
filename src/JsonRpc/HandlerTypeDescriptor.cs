using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using MediatR;

namespace OmniSharp.Extensions.JsonRpc
{
    [DebuggerDisplay("{" + nameof(Method) + "}")]
    internal class HandlerTypeDescriptor : IHandlerTypeDescriptor
    {
        public HandlerTypeDescriptor(Type handlerType)
        {
            var method = MethodAttribute.From(handlerType);
            Method = method.Method;
            Direction = method.Direction;
            if (handlerType.IsGenericTypeDefinition)
            {
                handlerType = handlerType.MakeGenericType(handlerType.GetTypeInfo().GenericTypeParameters[0].GetGenericParameterConstraints()[0]);
            }

            HandlerType = handlerType;
            InterfaceType = HandlerTypeDescriptorHelper.GetHandlerInterface(handlerType);

            // This allows for us to have derived types
            // We are making the assumption that interface given here
            // if a GTD will have a constraint on the first generic type parameter
            // that is the real base type for this interface.
            if (InterfaceType.IsGenericType)
            {
                ParamsType = InterfaceType.GetGenericArguments()[0];
            }
            else
            {
                ParamsType = typeof(EmptyRequest);
            }

            HasParamsType = ParamsType != null && ParamsType != typeof(EmptyRequest);
            IsNotification = typeof(IJsonRpcNotificationHandler).IsAssignableFrom(handlerType) || handlerType
                                                                                                 .GetInterfaces().Any(
                                                                                                      z =>
                                                                                                          z.IsGenericType && typeof(IJsonRpcNotificationHandler<>).IsAssignableFrom(
                                                                                                              z.GetGenericTypeDefinition()
                                                                                                          )
                                                                                                  );
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

using System;
using System.Diagnostics;
using System.Linq;
using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Shared
{
    [DebuggerDisplay("{Method}")]
    internal class HandlerDescriptor : IHandlerDescriptor, IDisposable
    {
        private readonly Action _disposeAction;

        public HandlerDescriptor(
            string method, IHandlerTypeDescriptor typeDescriptor, IJsonRpcHandler handler, Type handlerInterface, Type @params, Type response,
            RequestProcessType? requestProcessType, Action disposeAction
        )
        {
            _disposeAction = disposeAction;
            Handler = handler;
            ImplementationType = handler.GetType();
            Method = method;
            TypeDescriptor = typeDescriptor;
            HandlerType = handlerInterface;
            Params = @params;
            Response = response;
            HasReturnType = HandlerType.GetInterfaces().Any(
                @interface =>
                    @interface.IsGenericType &&
                    typeof(IRequestHandler<,>).IsAssignableFrom(@interface.GetGenericTypeDefinition())
            );

            IsDelegatingHandler = @params?.IsGenericType == true &&
                                  (
                                      typeof(DelegatingRequest<>).IsAssignableFrom(@params.GetGenericTypeDefinition()) ||
                                      typeof(DelegatingNotification<>).IsAssignableFrom(@params.GetGenericTypeDefinition())
                                  );

            IsNotification = typeof(IJsonRpcNotificationHandler).IsAssignableFrom(handlerInterface) || handlerInterface
                                                                                                      .GetInterfaces().Any(
                                                                                                           z =>
                                                                                                               z.IsGenericType && typeof(IJsonRpcNotificationHandler<>)
                                                                                                                  .IsAssignableFrom(z.GetGenericTypeDefinition())
                                                                                                       );
            IsRequest = !IsNotification;
            RequestProcessType = requestProcessType;
        }

        public IJsonRpcHandler Handler { get; }
        public bool IsNotification { get; }
        public bool IsRequest { get; }
        public Type HandlerType { get; }
        public Type ImplementationType { get; }
        public string Method { get; }
        public IHandlerTypeDescriptor TypeDescriptor { get; }
        public Type Params { get; }
        public Type Response { get; }
        public bool HasReturnType { get; }
        public bool IsDelegatingHandler { get; }
        public RequestProcessType? RequestProcessType { get; }

        public void Dispose() => _disposeAction();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reflection;
using System.Threading;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace OmniSharp.Extensions.JsonRpc
{


        [DebuggerDisplay("{Method}")]
        internal class HandlerInstance : IHandlerDescriptor, IDisposable
        {
            private readonly Action _disposeAction;

            public HandlerInstance(string method, IJsonRpcHandler handler, Type handlerInterface, Type @params, Type response, RequestProcessType? requestProcessType, Action disposeAction)
            {
                _disposeAction = disposeAction;
                Handler = handler;
                ImplementationType = handler.GetType();
                Method = method;
                HandlerType = handlerInterface;
                Params = @params;
                Response = response;
                HasReturnType = HandlerType.GetInterfaces().Any(@interface =>
                    @interface.IsGenericType &&
                    typeof(IRequestHandler<,>).IsAssignableFrom(@interface.GetGenericTypeDefinition())
                );

                IsDelegatingHandler = @params?.IsGenericType == true &&
                    (
                        typeof(DelegatingRequest<>).IsAssignableFrom(@params.GetGenericTypeDefinition()) ||
                        typeof(DelegatingNotification<>).IsAssignableFrom(@params.GetGenericTypeDefinition())
                    );

                IsNotification = typeof(IJsonRpcNotificationHandler).IsAssignableFrom(handlerInterface) || handlerInterface
                                     .GetInterfaces().Any(z =>
                                         z.IsGenericType && typeof(IJsonRpcNotificationHandler<>).IsAssignableFrom(z.GetGenericTypeDefinition()));
                IsRequest = !IsNotification;
                RequestProcessType = requestProcessType;
            }

            public IJsonRpcHandler Handler { get; }
            public bool IsNotification { get; }
            public bool IsRequest { get; }
            public Type HandlerType { get; }
            public Type ImplementationType { get; }
            public string Method { get; }
            public Type Params { get; }
            public Type Response { get; }
            public bool HasReturnType { get; }
            public bool IsDelegatingHandler { get; }
            public RequestProcessType? RequestProcessType { get; }

            public void Dispose()
            {
                _disposeAction();
            }
        }

        [DebuggerDisplay("{Method}")]
        internal class LinkedHandler : IHandlerDescriptor, IDisposable
        {
            private readonly IHandlerDescriptor _descriptor;
            private readonly Action _disposeAction;

            public LinkedHandler(string method, IHandlerDescriptor descriptor, Action disposeAction)
            {
                _descriptor = descriptor;
                _disposeAction = disposeAction;
                Method = method;
            }
            public string Method { get; }
            public Type HandlerType => _descriptor.HandlerType;

            public Type ImplementationType => _descriptor.ImplementationType;

            public Type Params => _descriptor.Params;

            public Type Response => _descriptor.Response;

            public bool HasReturnType => _descriptor.HasReturnType;

            public bool IsDelegatingHandler => _descriptor.IsDelegatingHandler;

            public IJsonRpcHandler Handler => _descriptor.Handler;

            public RequestProcessType? RequestProcessType => _descriptor.RequestProcessType;

            public void Dispose()
            {
                _disposeAction();
            }
        }


    public class HandlerCollection : IEnumerable<IHandlerDescriptor>, IHandlersManager
    {
        private readonly IServiceProvider _serviceProvider;
        private ImmutableArray<IHandlerDescriptor> _descriptors = ImmutableArray<IHandlerDescriptor>.Empty;

        public HandlerCollection(IJsonRpcHandlerCollection descriptions, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            descriptions.Populate(serviceProvider, this);
        }

        public IEnumerator<IHandlerDescriptor> GetEnumerator()
        {
            return _descriptors.AsEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void Remove(IJsonRpcHandler handler)
        {
            var descriptors = _descriptors.ToBuilder();
            foreach (var item in _descriptors.Where(instance => instance.Handler == handler))
            {
                descriptors.Remove(item);
            }

            ImmutableInterlocked.InterlockedExchange(ref _descriptors, descriptors.ToImmutableArray());
        }

        public IDisposable Add(params IJsonRpcHandler[] handlers)
        {
            var cd = new CompositeDisposable();
            foreach (var handler in handlers)
            {
                if (_descriptors.Any(z => z.Handler == handler)) continue;
                cd.Add(Add(GetMethodName(handler.GetType()), handler, null));
            }
            return cd;
        }

        public IDisposable Add(IJsonRpcHandler handler, JsonRpcHandlerOptions options)
        {
            return Add(GetMethodName(handler.GetType()), handler, options);
        }

        public IDisposable Add(string method, IJsonRpcHandler handler, JsonRpcHandlerOptions options)
        {
            var type = handler.GetType();
            var @interface = GetHandlerInterface(type);

            Type @params = null;
            Type response = null;
            if (@interface.GetTypeInfo().IsGenericType)
            {
                @params = @interface.GetTypeInfo().GetGenericArguments()[0];
                var requestInterface = @params.GetInterfaces()
                    .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRequest<>));
                if (requestInterface != null)
                {
                    response = requestInterface.GetGenericArguments()[0];
                }
            }

            var requestProcessType =
                options?.RequestProcessType ??
                type.GetCustomAttributes(true)
                .Concat(@interface.GetCustomAttributes(true))
                .OfType<ProcessAttribute>()
                .FirstOrDefault()?.Type;

            var descriptor = new HandlerInstance(method, handler, @interface, @params, response, requestProcessType, () => Remove(handler));
            ImmutableInterlocked.InterlockedExchange(ref _descriptors, _descriptors.Add(descriptor));
            return descriptor;
        }

        public IDisposable Add(JsonRpcHandlerFactory factory, JsonRpcHandlerOptions options) => Add(factory(_serviceProvider), options);

        public IDisposable Add(string method, JsonRpcHandlerFactory factory, JsonRpcHandlerOptions options) => Add(method, factory(_serviceProvider), options);

        public IDisposable Add(Type handlerType, JsonRpcHandlerOptions options) =>  Add(ActivatorUtilities.CreateInstance(_serviceProvider, handlerType) as IJsonRpcHandler, options);

        public IDisposable Add(string method, Type handlerType, JsonRpcHandlerOptions options) =>  Add(method, ActivatorUtilities.CreateInstance(_serviceProvider, handlerType) as IJsonRpcHandler, options);

        public IDisposable AddLink(string sourceMethod, string destinationMethod)
        {
            var source = _descriptors.FirstOrDefault(z => z.Method == sourceMethod);
            var descriptor = new LinkedHandler(destinationMethod, source, () => _descriptors.RemoveAll(z => z.Method == destinationMethod));
            ImmutableInterlocked.InterlockedExchange(ref _descriptors, _descriptors.Add(descriptor));
            return descriptor;
        }

        public bool ContainsHandler(Type type)
        {
            return _descriptors.Any(z => type.IsAssignableFrom(z.HandlerType));
        }

        public bool ContainsHandler(TypeInfo type)
        {
            return _descriptors.Any(z => type.IsAssignableFrom(z.HandlerType));
        }

        private static readonly Type[] HandlerTypes = {
            typeof(IJsonRpcNotificationHandler),
            typeof(IJsonRpcNotificationHandler<>),
            typeof(IJsonRpcRequestHandler<>),
            typeof(IJsonRpcRequestHandler<,>),
        };

        private string GetMethodName(Type type)
        {
            // Custom method
            var attribute = MethodAttribute.From(type.GetTypeInfo());

            // TODO: Log unknown method name
            if (attribute is null)
            {

            }

            return attribute.Method;
        }

        private bool IsValidInterface(Type type)
        {
            if (type.GetTypeInfo().IsGenericType)
            {
                return HandlerTypes.Contains(type.GetGenericTypeDefinition());
            }
            return HandlerTypes.Contains(type);
        }

        private Type GetHandlerInterface(Type type)
        {
            return type?.GetTypeInfo()
                .ImplementedInterfaces
                .First(IsValidInterface);
        }
    }
}

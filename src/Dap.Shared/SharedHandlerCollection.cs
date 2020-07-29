using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Server;

namespace OmniSharp.Extensions.DebugAdapter.Shared
{
    class DebugAdapterHandlerCollection : IEnumerable<IHandlerDescriptor>, IHandlersManager
    {
        internal readonly HashSet<HandlerDescriptor> _handlers = new HashSet<HandlerDescriptor>();

        public IEnumerator<IHandlerDescriptor> GetEnumerator()
        {
            return _handlers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IDisposable IHandlersManager.Add(IJsonRpcHandler handler, JsonRpcHandlerOptions options) => Add(new[] {handler}, options);

        IDisposable IHandlersManager.Add(string method, IJsonRpcHandler handler, JsonRpcHandlerOptions options) => Add(method, handler, options);

        IDisposable IHandlersManager.AddLink(string sourceMethod, string destinationMethod)
        {
            var source = _handlers.First(z => z.Method == sourceMethod);
            HandlerDescriptor descriptor = null;
            descriptor = GetDescriptor(
                destinationMethod,
                source.HandlerType,
                source.Handler,
                source.RequestProcessType.HasValue ? new JsonRpcHandlerOptions() {RequestProcessType = source.RequestProcessType.Value} : null,
                source.TypeDescriptor,
                source.HandlerType);
            _handlers.Add(descriptor);

            return descriptor;
        }

        public IDisposable Add(string method, IJsonRpcHandler handler, JsonRpcHandlerOptions options)
        {
            var descriptor = GetDescriptor(method, handler.GetType(), handler, options);
            _handlers.Add(descriptor);
            return new CompositeDisposable {descriptor};
        }

        public IDisposable Add(params IJsonRpcHandler[] handlers)
        {
            var cd = new CompositeDisposable();
            foreach (var handler in handlers)
            {
                if (cd.Any(z => Equals(z, handler))) continue;

                foreach (var (method, implementedInterface) in handler.GetType().GetTypeInfo()
                    .ImplementedInterfaces
                    .Select(x => (method: HandlerTypeDescriptorHelper.GetMethodName(x), implementedInterface: x))
                    .Distinct(new EqualityComparer())
                    .Where(x => !string.IsNullOrWhiteSpace(x.method))
                )
                {
                    var descriptor = GetDescriptor(method, implementedInterface, handler, null);
                    cd.Add(descriptor);
                    _handlers.Add(descriptor);
                }
            }

            return cd;
        }

        class EqualityComparer : IEqualityComparer<(string method, Type implementedInterface)>
        {
            public bool Equals((string method, Type implementedInterface) x, (string method, Type implementedInterface) y)
            {
                return x.method?.Equals(y.method) == true;
            }

            public int GetHashCode((string method, Type implementedInterface) obj)
            {
                return obj.method?.GetHashCode() ?? 0;
            }
        }

        private IDisposable Add(IJsonRpcHandler[] handlers, JsonRpcHandlerOptions options)
        {
            var cd = new CompositeDisposable();
            foreach (var handler in handlers)
            {
                if (cd.Any(z => Equals(z, handler))) continue;

                foreach (var (method, implementedInterface) in handler.GetType().GetTypeInfo()
                    .ImplementedInterfaces
                    .Select(x => (method: HandlerTypeDescriptorHelper.GetMethodName(x), implementedInterface: x))
                    .Where(x => !string.IsNullOrWhiteSpace(x.method)))
                {
                    var descriptor = GetDescriptor(method, implementedInterface, handler, options);
                    cd.Add(descriptor);
                    _handlers.Add(descriptor);
                }
            }

            return cd;
        }

        private HandlerDescriptor GetDescriptor(string method, Type handlerType, IJsonRpcHandler handler, JsonRpcHandlerOptions options)
        {
            var typeDescriptor = HandlerTypeDescriptorHelper.GetHandlerTypeDescriptor(method);
            var @interface = HandlerTypeDescriptorHelper.GetHandlerInterface(handlerType);

            return GetDescriptor(method, handlerType, handler, options, typeDescriptor, @interface);
        }

        private HandlerDescriptor GetDescriptor(string method, Type handlerType, IJsonRpcHandler handler, JsonRpcHandlerOptions options,
            IHandlerTypeDescriptor typeDescriptor,
            Type @interface)
        {
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
                typeDescriptor?.RequestProcessType ??
                handlerType.GetCustomAttributes(true)
                    .Concat(@interface.GetCustomAttributes(true))
                    .OfType<ProcessAttribute>()
                    .FirstOrDefault()?.Type;

            var descriptor = new HandlerDescriptor(
                method,
                typeDescriptor,
                handler,
                @interface,
                @params,
                response,
                requestProcessType,
                () => { _handlers.RemoveWhere(d => d.Handler == handler); });

            return descriptor;
        }

        public bool ContainsHandler(Type type)
        {
            return ContainsHandler(type.GetTypeInfo());
        }

        public bool ContainsHandler(TypeInfo typeInfo)
        {
            return this.Any(z => z.HandlerType.GetTypeInfo().IsAssignableFrom(typeInfo) || z.ImplementationType.GetTypeInfo().IsAssignableFrom(typeInfo));
        }
    }

    internal class DebugAdapterRequestRouter : RequestRouterBase<IHandlerDescriptor>
    {
        private readonly DebugAdapterHandlerCollection _collection;


        public DebugAdapterRequestRouter(DebugAdapterHandlerCollection collection, ISerializer serializer, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory)
            : base(serializer, serviceProvider, serviceScopeFactory, loggerFactory.CreateLogger<RequestRouter>())
        {
            _collection = collection;
        }

        public IDisposable Add(IJsonRpcHandler handler)
        {
            return _collection.Add(handler);
        }

        private IHandlerDescriptor FindDescriptor(IMethodWithParams instance)
        {
            return _collection.FirstOrDefault(x => x.Method == instance.Method);
        }

        public override IHandlerDescriptor GetDescriptor(Notification notification)
        {
            return FindDescriptor(notification);
        }

        public override IHandlerDescriptor GetDescriptor(Request request)
        {
            return FindDescriptor(request);
        }
    }
}

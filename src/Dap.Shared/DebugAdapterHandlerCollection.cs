using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Disposables;
using System.Reflection;
using System.Threading;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Shared
{
    class DebugAdapterHandlerCollection : IEnumerable<IHandlerDescriptor>, IHandlersManager
    {
        private ImmutableHashSet<HandlerDescriptor> _descriptors =  ImmutableHashSet<HandlerDescriptor>.Empty;
        private readonly IServiceProvider _serviceProvider;

        public IEnumerable<IHandlerDescriptor> Descriptors => _descriptors;

        public DebugAdapterHandlerCollection(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IEnumerator<IHandlerDescriptor> GetEnumerator()
        {
            return _descriptors.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IDisposable Add(IJsonRpcHandler handler, JsonRpcHandlerOptions options) => AddHandler(handler, options);
        public IDisposable Add(string method, IJsonRpcHandler handler, JsonRpcHandlerOptions options) => AddHandler(method, handler, options);
        public IDisposable Add(JsonRpcHandlerFactory factory, JsonRpcHandlerOptions options) => AddHandler(factory(_serviceProvider), options);
        public IDisposable Add(string method, JsonRpcHandlerFactory factory, JsonRpcHandlerOptions options) => AddHandler(method, factory(_serviceProvider), options);
        public IDisposable Add(Type handlerType, JsonRpcHandlerOptions options) => AddHandler(ActivatorUtilities.CreateInstance(_serviceProvider, handlerType) as IJsonRpcHandler, options);
        public IDisposable Add(string method, Type handlerType, JsonRpcHandlerOptions options) => AddHandler(method, ActivatorUtilities.CreateInstance(_serviceProvider, handlerType) as IJsonRpcHandler, options);

        IDisposable IHandlersManager.AddLink(string sourceMethod, string destinationMethod)
        {
            var source = _descriptors.First(z => z.Method == sourceMethod);
            HandlerDescriptor descriptor = null;
            descriptor = GetDescriptor(
                destinationMethod,
                source.HandlerType,
                source.Handler,
                source.RequestProcessType.HasValue ? new JsonRpcHandlerOptions() {RequestProcessType = source.RequestProcessType.Value} : null,
                source.TypeDescriptor,
                source.HandlerType);
            Interlocked.Exchange(ref _descriptors, _descriptors.Add(descriptor));

            return descriptor;
        }

        public IDisposable Add(params IJsonRpcHandler[] handlers)
        {
            var cd = new CompositeDisposable();
            foreach (var handler in handlers)
            {
                cd.Add(AddHandler(handler, null));
            }
            return cd;
        }

        public IDisposable Add(params JsonRpcHandlerFactory[] handlerFactories)
        {
            var cd = new CompositeDisposable();
            foreach (var handlerFactory in handlerFactories)
            {
                cd.Add(AddHandler(handlerFactory(_serviceProvider), null));
            }
            return cd;
        }

        public IDisposable Add(params Type[] handlerTypes)
        {
            var cd = new CompositeDisposable();
            foreach (var handlerType in handlerTypes)
            {
                cd.Add(AddHandler(ActivatorUtilities.CreateInstance(_serviceProvider, handlerType) as IJsonRpcHandler, null));
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

        private IDisposable AddHandler(string method, IJsonRpcHandler handler, JsonRpcHandlerOptions options)
        {
            var descriptor = GetDescriptor(method, handler.GetType(), handler, options);
            Interlocked.Exchange(ref _descriptors, _descriptors.Add(descriptor));
            return descriptor;
        }

        private CompositeDisposable AddHandler(IJsonRpcHandler handler, JsonRpcHandlerOptions options)
        {
            var cd = new CompositeDisposable();
            foreach (var (method, implementedInterface) in handler.GetType().GetTypeInfo()
                .ImplementedInterfaces
                .Select(x => (method: HandlerTypeDescriptorHelper.GetMethodName(x), implementedInterface: x))
                .Distinct(new EqualityComparer())
                .Where(x => !string.IsNullOrWhiteSpace(x.method))
            )
            {
                var descriptor = GetDescriptor(method, implementedInterface, handler, options);
                cd.Add(descriptor);
                Interlocked.Exchange(ref _descriptors, _descriptors.Add(descriptor));
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
                () => {
                    var descriptors = _descriptors.ToBuilder();
                    foreach (var descriptor in _descriptors)
                    {
                        if (descriptor.Handler != handler) continue;
                        descriptors.Remove(descriptor);
                    }

                    Interlocked.Exchange(ref _descriptors, descriptors.ToImmutable());
                });

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
}

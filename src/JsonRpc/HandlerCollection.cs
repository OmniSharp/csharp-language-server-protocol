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

namespace OmniSharp.Extensions.JsonRpc
{
    class HandlerCollection : IHandlersManager, IEnumerable<IHandlerDescriptor>
    {
        private readonly IServiceProvider _serviceProvider;
        private ImmutableArray<IHandlerDescriptor> _descriptors = ImmutableArray<IHandlerDescriptor>.Empty;

        public IEnumerable<IHandlerDescriptor> Descriptors => _descriptors;

        public HandlerCollection(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
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
                cd.Add(Add(HandlerTypeDescriptorHelper.GetMethodName(handler.GetType()), handler, null));
            }
            return cd;
        }

        public IDisposable Add(IJsonRpcHandler handler, JsonRpcHandlerOptions options)
        {
            return Add(HandlerTypeDescriptorHelper.GetMethodName(handler.GetType()), handler, options);
        }

        public IDisposable Add(string method, IJsonRpcHandler handler, JsonRpcHandlerOptions options)
        {
            var type = handler.GetType();
            var @interface = HandlerTypeDescriptorHelper.GetHandlerInterface(type);

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

        public IEnumerator<IHandlerDescriptor> GetEnumerator()
        {
            return ((IEnumerable<IHandlerDescriptor>)_descriptors).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_descriptors).GetEnumerator();
        }
    }
}

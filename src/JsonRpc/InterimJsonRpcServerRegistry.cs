using System;
using Microsoft.Extensions.DependencyInjection;

namespace OmniSharp.Extensions.JsonRpc
{
    public class InterimJsonRpcServerRegistry<T> : JsonRpcCommonMethodsBase<T> where T : IJsonRpcHandlerRegistry<T>
    {
        protected readonly IServiceProvider _serviceProvider;
        private readonly IHandlersManager _handlersManager;

        public InterimJsonRpcServerRegistry(IServiceProvider serviceProvider, IHandlersManager handlersManager)
        {
            _serviceProvider = serviceProvider;
            _handlersManager = handlersManager;
        }

        public sealed override T AddHandler(string method, IJsonRpcHandler handler, JsonRpcHandlerOptions options)
        {
            _handlersManager.Add(method, handler, options);
            return (T) (object) this;
        }

        public sealed override T AddHandler(string method, Func<IServiceProvider, IJsonRpcHandler> handlerFunc, JsonRpcHandlerOptions options)
        {
            _handlersManager.Add(method, handlerFunc(_serviceProvider), options);
            return (T) (object) this;
        }

        public sealed override T AddHandler<THandler>(Func<IServiceProvider, THandler> handlerFunc, JsonRpcHandlerOptions options)
        {
            _handlersManager.Add(handlerFunc(_serviceProvider), options);
            return (T) (object) this;
        }

        public sealed override T AddHandlers(params IJsonRpcHandler[] handlers)
        {
            foreach (var handler in handlers)
            {
                _handlersManager.Add(handler, null);
            }

            return (T) (object) this;
        }

        public sealed override T AddHandler<THandler>(THandler handler, JsonRpcHandlerOptions options)
        {
            _handlersManager.Add(handler, options);
            return (T) (object) this;
        }

        public sealed override T AddHandler<THandler>(JsonRpcHandlerOptions options)
        {
            return AddHandler(typeof(THandler), options);
        }

        public sealed override T AddHandler<THandler>(string method, JsonRpcHandlerOptions options)
        {
            return AddHandler(method, typeof(THandler), options);
        }

        public sealed override T AddHandler(Type type, JsonRpcHandlerOptions options)
        {
            _handlersManager.Add(ActivatorUtilities.CreateInstance(_serviceProvider, type) as IJsonRpcHandler, options);
            return (T) (object) this;
        }

        public sealed override T AddHandler(string method, Type type, JsonRpcHandlerOptions options)
        {
            _handlersManager.Add(method, ActivatorUtilities.CreateInstance(_serviceProvider, type) as IJsonRpcHandler, options);
            return (T) (object) this;
        }
    }
}

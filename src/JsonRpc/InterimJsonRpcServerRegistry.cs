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

        public sealed override T AddHandler(string method, IJsonRpcHandler handler)
        {
            _handlersManager.Add(method, handler);
            return (T) (object) this;
        }

        public sealed override T AddHandler(string method, Func<IServiceProvider, IJsonRpcHandler> handlerFunc)
        {
            _handlersManager.Add(method, handlerFunc(_serviceProvider));
            return (T) (object) this;
        }

        public sealed override T AddHandler<THandler>(Func<IServiceProvider, THandler> handlerFunc)
        {
            _handlersManager.Add(handlerFunc(_serviceProvider));
            return (T) (object) this;
        }

        public sealed override T AddHandlers(params IJsonRpcHandler[] handlers)
        {
            foreach (var handler in handlers)
            {
                _handlersManager.Add(handler);
            }

            return (T) (object) this;
        }

        public sealed override T AddHandler<THandler>(THandler handler)
        {
            _handlersManager.Add(handler);
            return (T) (object) this;
        }

        public sealed override T AddHandler<THandler>()
        {
            return AddHandler(typeof(THandler));
        }

        public sealed override T AddHandler<THandler>(string method)
        {
            return AddHandler(method, typeof(THandler));
        }

        public sealed override T AddHandler(Type type)
        {
            _handlersManager.Add(ActivatorUtilities.CreateInstance(_serviceProvider, type) as IJsonRpcHandler);
            return (T) (object) this;
        }

        public sealed override T AddHandler(string method, Type type)
        {
            _handlersManager.Add(method, ActivatorUtilities.CreateInstance(_serviceProvider, type) as IJsonRpcHandler);
            return (T) (object) this;
        }
    }
}

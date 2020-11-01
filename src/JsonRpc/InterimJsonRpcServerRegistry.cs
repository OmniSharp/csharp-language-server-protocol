using System;

namespace OmniSharp.Extensions.JsonRpc
{
    public class InterimJsonRpcServerRegistry<T> : JsonRpcCommonMethodsBase<T> where T : IJsonRpcHandlerRegistry<T>
    {
        private readonly IHandlersManager _handlersManager;

        public InterimJsonRpcServerRegistry(IHandlersManager handlersManager) => _handlersManager = handlersManager;

        public sealed override T AddHandler(string method, IJsonRpcHandler handler, JsonRpcHandlerOptions? options = null)
        {
            _handlersManager.Add(method, handler, options);
            return (T) (object) this;
        }

        public sealed override T AddHandler(string method, JsonRpcHandlerFactory handlerFunc, JsonRpcHandlerOptions? options = null)
        {
            _handlersManager.Add(method, handlerFunc, options);
            return (T) (object) this;
        }

        public sealed override T AddHandler(JsonRpcHandlerFactory handlerFunc, JsonRpcHandlerOptions? options = null)
        {
            _handlersManager.Add(handlerFunc, options);
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

        public sealed override T AddHandler(IJsonRpcHandler handler, JsonRpcHandlerOptions? options = null)
        {
            _handlersManager.Add(handler, options);
            return (T) (object) this;
        }

        public sealed override T AddHandler<THandler>(JsonRpcHandlerOptions? options = null) => AddHandler(typeof(THandler), options);

        public sealed override T AddHandler<THandler>(string method, JsonRpcHandlerOptions? options = null) => AddHandler(method, typeof(THandler), options);

        public sealed override T AddHandler(Type type, JsonRpcHandlerOptions? options = null)
        {
            _handlersManager.Add(type, options);
            return (T) (object) this;
        }

        public sealed override T AddHandler(string method, Type type, JsonRpcHandlerOptions? options = null)
        {
            _handlersManager.Add(method, type, options);
            return (T) (object) this;
        }

        public sealed override T AddHandlerLink(string fromMethod, string toMethod)
        {
            _handlersManager.AddLink(fromMethod, toMethod);
            return (T) (object) this;
        }
    }
}

using System;
using Microsoft.Extensions.DependencyInjection;

namespace OmniSharp.Extensions.JsonRpc
{
    public abstract class JsonRpcOptionsRegistryBase<T> : JsonRpcCommonMethodsBase<T> where T : IJsonRpcHandlerRegistry<T>
    {
        internal IServiceCollection Services { get; } = new ServiceCollection()
            .AddLogging()
            .AddOptions();
        public IJsonRpcHandlerCollection Handlers { get; } = new JsonRpcHandlerCollection();

        public T WithServices(Action<IServiceCollection> servicesAction)
        {
            servicesAction(Services);
            return (T) (object) this;
        }

        #region AddHandler

        public sealed override T AddHandler(string method, IJsonRpcHandler handler, JsonRpcHandlerOptions options = null)
        {
            Handlers.Add(JsonRpcHandlerDescription.Named(method, handler, options));
            return (T) (object) this;
        }

        public sealed override T AddHandler(string method, JsonRpcHandlerFactory handlerFunc, JsonRpcHandlerOptions options = null)
        {
            Handlers.Add(JsonRpcHandlerDescription.Named(method, handlerFunc, options));
            return (T) (object) this;
        }

        public sealed override T AddHandler(IJsonRpcHandler handler, JsonRpcHandlerOptions options = null)
        {
            Handlers.Add(JsonRpcHandlerDescription.Named(null, handler, options));
            return (T) (object) this;
        }

        public sealed override T AddHandler(JsonRpcHandlerFactory handlerFunc, JsonRpcHandlerOptions options = null)
        {
            Handlers.Add(JsonRpcHandlerDescription.Named(null, handlerFunc, options));
            return (T) (object) this;
        }

        public sealed override T AddHandlers(params IJsonRpcHandler[] handlers)
        {
            foreach (var item in handlers)
            {
                Handlers.Add(JsonRpcHandlerDescription.Infer(item));
            }

            return (T) (object) this;
        }

        public sealed override T AddHandler<THandler>(JsonRpcHandlerOptions options = null)
        {
            return AddHandler(typeof(THandler), options);
        }

        public sealed override T AddHandler<THandler>(string method, JsonRpcHandlerOptions options = null)
        {
            return AddHandler(method, typeof(THandler), options);
        }

        public sealed override T AddHandler(Type type, JsonRpcHandlerOptions options = null)
        {
            Handlers.Add(JsonRpcHandlerDescription.Infer(type, options));
            return (T) (object) this;
        }

        public sealed override T AddHandler(string method, Type type, JsonRpcHandlerOptions options = null)
        {
            Handlers.Add(JsonRpcHandlerDescription.Named(method, type, options));
            return (T) (object) this;
        }

        #endregion
    }
}

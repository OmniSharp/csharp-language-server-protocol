using System;
using Microsoft.Extensions.DependencyInjection;

namespace OmniSharp.Extensions.JsonRpc
{
    public abstract class JsonRpcOptionsRegistryBase<T> : JsonRpcCommonMethodsBase<T> where T : IJsonRpcHandlerRegistry<T>
    {
        public IServiceCollection Services { get; set; } = new ServiceCollection()
                                                          .AddOptions()
                                                          .AddLogging();

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

        public sealed override T AddHandler<THandler>(JsonRpcHandlerOptions options = null) => AddHandler(typeof(THandler), options);

        public sealed override T AddHandler<THandler>(string method, JsonRpcHandlerOptions options = null) => AddHandler(method, typeof(THandler), options);

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

        public sealed override T AddHandlerLink(string sourceMethod, string destinationMethod)
        {
            Handlers.Add(JsonRpcHandlerDescription.Link(sourceMethod, destinationMethod));
            return (T) (object) this;
        }

        #endregion
    }
}

using System;
using Microsoft.Extensions.DependencyInjection;

namespace OmniSharp.Extensions.JsonRpc
{
    public abstract class JsonRpcOptionsRegistryBase<T> : JsonRpcCommonMethodsBase<T> where T : IJsonRpcHandlerRegistry<T>
    {
        public IServiceCollection Services { get; set;  } = new ServiceCollection();

        #region AddHandler

        public sealed override T AddHandler(string method, IJsonRpcHandler handler, JsonRpcHandlerOptions options = null)
        {
            Services.AddSingleton(Named(method, handler, options));
            return (T) (object) this;
        }

        public sealed override T AddHandler(string method, Func<IServiceProvider, IJsonRpcHandler> handlerFunc, JsonRpcHandlerOptions options = null)
        {
            Services.AddSingleton(Named(method, handlerFunc, options));
            return (T) (object) this;
        }

        public sealed override T AddHandler<THandler>(Func<IServiceProvider, THandler> handlerFunc, JsonRpcHandlerOptions options = null)
        {
            Services.AddSingleton(Unnamed(_ => handlerFunc(_), options));
            return (T) (object) this;
        }

        public sealed override T AddHandlers(params IJsonRpcHandler[] handlers)
        {
            foreach (var item in handlers)
            {
                Services.AddSingleton(item);
            }

            return (T) (object) this;
        }

        public sealed override T AddHandler<THandler>(JsonRpcHandlerOptions options = null)
        {
            return AddHandler(typeof(THandler), options);
        }

        public sealed override T AddHandler<THandler>(THandler handler, JsonRpcHandlerOptions options = null)
        {
            Services.AddSingleton(Unnamed(handler, options));
            return (T) (object) this;
        }

        public sealed override T AddHandler<THandler>(string method, JsonRpcHandlerOptions options = null)
        {
            return AddHandler(method, typeof(THandler), options);
        }

        public sealed override T AddHandler(Type type, JsonRpcHandlerOptions options = null)
        {
            Services.AddSingleton(Unnamed(type, options));
            return (T) (object) this;
        }

        public sealed override T AddHandler(string method, Type type, JsonRpcHandlerOptions options = null)
        {
            Services.AddSingleton(Named(method, type, options));
            return (T) (object) this;
        }

        private Func<IServiceProvider, IJsonRpcHandler> Unnamed(IJsonRpcHandler handler, JsonRpcHandlerOptions options)
        {
            return _ => {
                _.GetRequiredService<IHandlersManager>().Add( handler, options);
                return handler;
            };
        }

        private Func<IServiceProvider, IJsonRpcHandler> Unnamed(Func<IServiceProvider, IJsonRpcHandler> factory, JsonRpcHandlerOptions options)
        {
            return _ => {
                var instance = factory(_);
                _.GetRequiredService<IHandlersManager>().Add( instance, options);
                return instance;
            };
        }

        private Func<IServiceProvider, IJsonRpcHandler> Unnamed(Type handlerType, JsonRpcHandlerOptions options)
        {
            return _ => {
                var instance = ActivatorUtilities.CreateInstance(_, handlerType) as IJsonRpcHandler;
                _.GetRequiredService<IHandlersManager>().Add( instance, options);
                return instance;
            };
        }

        private Func<IServiceProvider, IJsonRpcHandler> Named(string method, IJsonRpcHandler handler, JsonRpcHandlerOptions options)
        {
            return _ => {
                _.GetRequiredService<IHandlersManager>().Add(method, handler, options);
                return handler;
            };
        }

        private Func<IServiceProvider, IJsonRpcHandler> Named(string method, Func<IServiceProvider, IJsonRpcHandler> factory, JsonRpcHandlerOptions options)
        {
            return _ => {
                var instance = factory(_);
                _.GetRequiredService<IHandlersManager>().Add(method, instance, options);
                return instance;
            };
        }

        private Func<IServiceProvider, IJsonRpcHandler> Named(string method, Type handlerType, JsonRpcHandlerOptions options)
        {
            return _ => {
                var instance = ActivatorUtilities.CreateInstance(_, handlerType) as IJsonRpcHandler;
                _.GetRequiredService<IHandlersManager>().Add(method, instance, options);
                return instance;
            };
        }

        #endregion
    }
}

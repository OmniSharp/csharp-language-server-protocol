using System;
using Microsoft.Extensions.DependencyInjection;

namespace OmniSharp.Extensions.JsonRpc
{
    public abstract class JsonRpcOptionsRegistryBase<T> : JsonRpcCommonMethodsBase<T> where T : IJsonRpcHandlerRegistry<T>
    {
        public IServiceCollection Services { get; set;  } = new ServiceCollection();

        #region AddHandler

        public sealed override T AddHandler(string method, IJsonRpcHandler handler)
        {
            Services.AddSingleton(Named(method, handler));
            return (T) (object) this;
        }

        public sealed override T AddHandler(string method, Func<IServiceProvider, IJsonRpcHandler> handlerFunc)
        {
            Services.AddSingleton(Named(method, handlerFunc));
            return (T) (object) this;
        }

        public sealed override T AddHandler<THandler>(Func<IServiceProvider, THandler> handlerFunc)
        {
            Services.AddSingleton(handlerFunc);
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

        public sealed override T AddHandler<THandler>()
        {
            return AddHandler(typeof(THandler));
        }

        public sealed override T AddHandler<THandler>(THandler handler)
        {
            Services.AddSingleton(typeof(IJsonRpcHandler), handler);
            return (T) (object) this;
        }

        public sealed override T AddHandler<THandler>(string method)
        {
            return AddHandler(method, typeof(THandler));
        }

        public sealed override T AddHandler(Type type)
        {
            Services.AddSingleton(typeof(IJsonRpcHandler), type);
            return (T) (object) this;
        }

        public sealed override T AddHandler(string method, Type type)
        {
            Services.AddSingleton(Named(method, type));
            return (T) (object) this;
        }

        private Func<IServiceProvider, IJsonRpcHandler> Named(string method, IJsonRpcHandler handler)
        {
            return _ => {
                _.GetRequiredService<IHandlersManager>().Add(method, handler);
                return handler;
            };
        }

        private Func<IServiceProvider, IJsonRpcHandler> Named(string method, Func<IServiceProvider, IJsonRpcHandler> factory)
        {
            return _ => {
                var instance = factory(_);
                _.GetRequiredService<IHandlersManager>().Add(method, instance);
                return instance;
            };
        }

        private Func<IServiceProvider, IJsonRpcHandler> Named(string method, Type handlerType)
        {
            return _ => {
                var instance = ActivatorUtilities.CreateInstance(_, handlerType) as IJsonRpcHandler;
                _.GetRequiredService<IHandlersManager>().Add(method, instance);
                return instance;
            };
        }

        #endregion
    }
}

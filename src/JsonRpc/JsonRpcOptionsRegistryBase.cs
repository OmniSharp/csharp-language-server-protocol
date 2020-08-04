using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IJsonRpcHandlerCollection : IEnumerable<JsonRpcHandlerDescription>
    {
        IJsonRpcHandlerCollection Add(JsonRpcHandlerDescription description);
    }

    public delegate IJsonRpcHandler JsonRpcHandlerFactory(IServiceProvider serviceProvider);

    class JsonRpcHandlerCollection : IJsonRpcHandlerCollection
    {
        private List<JsonRpcHandlerDescription> _descriptions { get; } = new List<JsonRpcHandlerDescription>();
        public IEnumerator<JsonRpcHandlerDescription> GetEnumerator() => _descriptions.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IJsonRpcHandlerCollection Add(JsonRpcHandlerDescription description)
        {
            _descriptions.Add(description);
            return this;
        }
    }

    public static class JsonRpcHandlerCollectionExtensions
    {
        public static void Populate(this IJsonRpcHandlerCollection collection, IServiceProvider serviceProvider, IHandlersManager handlersManager)
        {
            foreach (var item in collection)
            {
                switch (item)
                {
                    case JsonRpcHandlerFactoryDescription factory when string.IsNullOrWhiteSpace(factory.Method):
                        handlersManager.Add(factory.HandlerFactory(serviceProvider), factory.Options);
                        continue;
                    case JsonRpcHandlerFactoryDescription factory:
                        handlersManager.Add(factory.Method, factory.HandlerFactory(serviceProvider), factory.Options);
                        continue;
                    case JsonRpcHandlerTypeDescription type when string.IsNullOrWhiteSpace(type.Method):
                        handlersManager.Add(ActivatorUtilities.CreateInstance(serviceProvider, type.HandlerType) as IJsonRpcHandler, type.Options);
                        continue;
                    case JsonRpcHandlerTypeDescription type:
                        handlersManager.Add(type.Method, ActivatorUtilities.CreateInstance(serviceProvider, type.HandlerType) as IJsonRpcHandler, type.Options);
                        continue;
                    case JsonRpcHandlerInstanceDescription instance when string.IsNullOrWhiteSpace(instance.Method):
                        handlersManager.Add(instance.HandlerInstance, instance.Options);
                        continue;
                    case JsonRpcHandlerInstanceDescription instance:
                        handlersManager.Add(instance.Method, instance.HandlerInstance, instance.Options);
                        continue;
                    case JsonRpcHandlerLinkDescription link:
                        handlersManager.AddLink(link.Method, link.LinkToMethod);
                        continue;
                }
            }
        }
    }

    public abstract class JsonRpcHandlerDescription
    {
        protected JsonRpcHandlerDescription(JsonRpcHandlerOptions options)
        {
            Options = options;
        }

        public JsonRpcHandlerOptions Options { get; }


        public static JsonRpcHandlerDescription Infer(Type handlerType, JsonRpcHandlerOptions options = null)
        {
            return new JsonRpcHandlerTypeDescription(null, handlerType, options);
        }

        public static JsonRpcHandlerDescription Named(string method, Type handlerType, JsonRpcHandlerOptions options = null)
        {
            return new JsonRpcHandlerTypeDescription(method, handlerType, options);
        }

        public static JsonRpcHandlerDescription Infer(IJsonRpcHandler handlerInstance, JsonRpcHandlerOptions options = null)
        {
            return new JsonRpcHandlerInstanceDescription(null, handlerInstance, options);
        }

        public static JsonRpcHandlerDescription Named(string method, IJsonRpcHandler handlerInstance, JsonRpcHandlerOptions options = null)
        {
            return new JsonRpcHandlerInstanceDescription(method, handlerInstance, options);
        }

        public static JsonRpcHandlerDescription Infer(JsonRpcHandlerFactory handlerFactory, JsonRpcHandlerOptions options = null)
        {
            return new JsonRpcHandlerFactoryDescription(null, handlerFactory, options);
        }

        public static JsonRpcHandlerDescription Named(string method, JsonRpcHandlerFactory handlerFactory, JsonRpcHandlerOptions options = null)
        {
            return new JsonRpcHandlerFactoryDescription(method, handlerFactory, options);
        }

        public static JsonRpcHandlerDescription Link(string method, string methodToLink)
        {
            return new JsonRpcHandlerLinkDescription(method, methodToLink);
        }
    }

    public class JsonRpcHandlerLinkDescription : JsonRpcHandlerDescription
    {
        public JsonRpcHandlerLinkDescription(string method, string linkToMethod) : base(new JsonRpcHandlerOptions())
        {
            Method = method;
            LinkToMethod = linkToMethod;
        }

        public string Method { get; }
        public string LinkToMethod { get; }
    }

    public class JsonRpcHandlerInstanceDescription : JsonRpcHandlerDescription
    {
        public JsonRpcHandlerInstanceDescription(string method, IJsonRpcHandler handlerInstance, JsonRpcHandlerOptions options): base(options)
        {
            Method = method;
            HandlerInstance = handlerInstance;
        }

        public string Method { get; }
        public IJsonRpcHandler HandlerInstance { get; }
    }

    public class JsonRpcHandlerTypeDescription : JsonRpcHandlerDescription
    {
        public JsonRpcHandlerTypeDescription(string method, Type handlerType, JsonRpcHandlerOptions options): base(options)
        {
            Method = method;
            HandlerType = handlerType;
        }

        public string Method { get; }
        public Type HandlerType { get; }
    }

    public class JsonRpcHandlerFactoryDescription : JsonRpcHandlerDescription
    {
        public JsonRpcHandlerFactoryDescription(string method, JsonRpcHandlerFactory handlerFactory, JsonRpcHandlerOptions options): base(options)
        {
            Method = method;
            HandlerFactory = handlerFactory;
        }

        public string Method { get; }
        public JsonRpcHandlerFactory HandlerFactory { get; }
    }

    public abstract class JsonRpcOptionsRegistryBase<T> : JsonRpcCommonMethodsBase<T> where T : IJsonRpcHandlerRegistry<T>
    {
        public IJsonRpcHandlerCollection Handlers { get; } = new JsonRpcHandlerCollection();

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

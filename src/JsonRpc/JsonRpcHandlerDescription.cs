using System;

namespace OmniSharp.Extensions.JsonRpc
{
    public abstract class JsonRpcHandlerDescription
    {
        protected JsonRpcHandlerDescription(JsonRpcHandlerOptions? options) => Options = options;

        public JsonRpcHandlerOptions? Options { get; }


        public static JsonRpcHandlerDescription Infer(Type handlerType, JsonRpcHandlerOptions? options = null) => new JsonRpcHandlerTypeDescription(null, handlerType, options);

        public static JsonRpcHandlerDescription Named(string method, Type handlerType, JsonRpcHandlerOptions? options = null) =>
            new JsonRpcHandlerTypeDescription(method, handlerType, options);

        public static JsonRpcHandlerDescription Infer(IJsonRpcHandler handlerInstance, JsonRpcHandlerOptions? options = null) =>
            new JsonRpcHandlerInstanceDescription(null, handlerInstance, options);

        public static JsonRpcHandlerDescription Named(string method, IJsonRpcHandler handlerInstance, JsonRpcHandlerOptions? options = null) =>
            new JsonRpcHandlerInstanceDescription(method, handlerInstance, options);

        public static JsonRpcHandlerDescription Infer(JsonRpcHandlerFactory handlerFactory, JsonRpcHandlerOptions? options = null) =>
            new JsonRpcHandlerFactoryDescription(null, handlerFactory, options);

        public static JsonRpcHandlerDescription Named(string method, JsonRpcHandlerFactory handlerFactory, JsonRpcHandlerOptions? options = null) =>
            new JsonRpcHandlerFactoryDescription(method, handlerFactory, options);

        public static JsonRpcHandlerDescription Link(string method, string methodToLink) => new JsonRpcHandlerLinkDescription(method, methodToLink);
    }
}

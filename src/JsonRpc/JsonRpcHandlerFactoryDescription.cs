namespace OmniSharp.Extensions.JsonRpc
{
    public class JsonRpcHandlerFactoryDescription : JsonRpcHandlerDescription
    {
        public JsonRpcHandlerFactoryDescription(string? method, JsonRpcHandlerFactory handlerFactory, JsonRpcHandlerOptions? options) : base(options)
        {
            Method = method;
            HandlerFactory = handlerFactory;
        }

        public string? Method { get; }
        public JsonRpcHandlerFactory HandlerFactory { get; }
    }
}

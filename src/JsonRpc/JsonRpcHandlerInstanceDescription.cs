namespace OmniSharp.Extensions.JsonRpc
{
    public class JsonRpcHandlerInstanceDescription : JsonRpcHandlerDescription
    {
        public JsonRpcHandlerInstanceDescription(string? method, IJsonRpcHandler handlerInstance, JsonRpcHandlerOptions? options) : base(options)
        {
            Method = method;
            HandlerInstance = handlerInstance;
        }

        public string? Method { get; }
        public IJsonRpcHandler HandlerInstance { get; }
    }
}

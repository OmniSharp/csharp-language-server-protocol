using System;

namespace OmniSharp.Extensions.JsonRpc
{
    public class JsonRpcHandlerTypeDescription : JsonRpcHandlerDescription
    {
        public JsonRpcHandlerTypeDescription(string? method, Type handlerType, JsonRpcHandlerOptions? options) : base(options)
        {
            Method = method;
            HandlerType = handlerType;
        }

        public string? Method { get; }
        public Type HandlerType { get; }
    }
}

using System;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IHandlersManager
    {
        IDisposable Add(IJsonRpcHandler handler, JsonRpcHandlerOptions options);
        IDisposable Add(string method, IJsonRpcHandler handler, JsonRpcHandlerOptions options);
        IDisposable Add(JsonRpcHandlerFactory factory, JsonRpcHandlerOptions options);
        IDisposable Add(string method, JsonRpcHandlerFactory factory, JsonRpcHandlerOptions options);
        IDisposable Add(Type handlerType, JsonRpcHandlerOptions options);
        IDisposable Add(string method, Type handlerType, JsonRpcHandlerOptions options);
        IDisposable AddLink(string sourceMethod, string destinationMethod);
    }
}

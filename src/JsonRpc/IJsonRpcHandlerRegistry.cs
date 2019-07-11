using System;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IJsonRpcHandlerRegistry
    {
        IDisposable AddHandler(string method, IJsonRpcHandler handler);
        IDisposable AddHandler(string method, Func<IServiceProvider, IJsonRpcHandler> handlerFunc);
        IDisposable AddHandlers(params IJsonRpcHandler[] handlers);
        IDisposable AddHandler<T>() where T : IJsonRpcHandler;
    }
}

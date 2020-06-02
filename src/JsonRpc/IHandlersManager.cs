using System;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IHandlersManager
    {
        IDisposable Add(IJsonRpcHandler handler, JsonRpcHandlerOptions options);
        IDisposable Add(string method, IJsonRpcHandler handler, JsonRpcHandlerOptions options);
    }
}

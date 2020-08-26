using System;
using System.Collections.Generic;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IHandlersManager
    {
        IDisposable Add(IJsonRpcHandler handler, JsonRpcHandlerOptions? options = null);
        IDisposable Add(string method, IJsonRpcHandler handler, JsonRpcHandlerOptions? options = null);
        IDisposable Add(JsonRpcHandlerFactory factory, JsonRpcHandlerOptions? options = null);
        IDisposable Add(string method, JsonRpcHandlerFactory factory, JsonRpcHandlerOptions? options = null);
        IDisposable Add(Type handlerType, JsonRpcHandlerOptions? options = null);
        IDisposable Add(string method, Type handlerType, JsonRpcHandlerOptions? options = null);
        IDisposable AddLink(string fromMethod, string toMethod);
        IEnumerable<IHandlerDescriptor> Descriptors { get; }
    }
}

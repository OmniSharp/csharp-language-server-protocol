using System;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IHandlersManager
    {
        IDisposable Add(IJsonRpcHandler handler);
        IDisposable Add(string method, IJsonRpcHandler handler);
    }
}
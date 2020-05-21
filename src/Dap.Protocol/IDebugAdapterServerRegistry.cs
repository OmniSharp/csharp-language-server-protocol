using System;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    public interface IDebugAdapterServerRegistry : IJsonRpcHandlerRegistry
    {
        IDisposable AddHandler<T>(Func<IServiceProvider, T> handlerFunc) where T : IJsonRpcHandler;
    }
}

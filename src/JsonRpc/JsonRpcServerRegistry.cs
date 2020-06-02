using System;

namespace OmniSharp.Extensions.JsonRpc
{
    class JsonRpcServerRegistry : InterimJsonRpcServerRegistry<IJsonRpcServerRegistry>, IJsonRpcServerRegistry
    {
        public JsonRpcServerRegistry(IServiceProvider serviceProvider, CompositeHandlersManager handlersManager) : base(serviceProvider, handlersManager)
        {
        }
    }
}
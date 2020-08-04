using System;

namespace OmniSharp.Extensions.JsonRpc
{
    class JsonRpcServerRegistry : InterimJsonRpcServerRegistry<IJsonRpcServerRegistry>, IJsonRpcServerRegistry
    {
        public JsonRpcServerRegistry(CompositeHandlersManager handlersManager) : base(handlersManager)
        {
        }
    }
}

using OmniSharp.Extensions.DebugAdapter.Protocol.Client;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Client
{
    class DebugAdapterClientRegistry : InterimJsonRpcServerRegistry<IDebugAdapterClientRegistry>, IDebugAdapterClientRegistry
    {
        public DebugAdapterClientRegistry(CompositeHandlersManager handlersManager) : base(handlersManager)
        {
        }
    }
}

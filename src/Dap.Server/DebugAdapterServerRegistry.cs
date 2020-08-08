using OmniSharp.Extensions.DebugAdapter.Protocol.Server;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Server
{
    class DebugAdapterServerRegistry : InterimJsonRpcServerRegistry<IDebugAdapterServerRegistry>, IDebugAdapterServerRegistry
    {
        public DebugAdapterServerRegistry(CompositeHandlersManager handlersManager) : base(handlersManager)
        {
        }
    }
}

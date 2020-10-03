using DryIoc;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Server;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Server
{
    class DefaultDebugAdapterServerFacade : DebugAdapterProtocolProxy, IDebugAdapterServerFacade
    {
        public DefaultDebugAdapterServerFacade(IResponseRouter requestRouter, IResolverContext resolverContext, IDebugAdapterProtocolSettings debugAdapterProtocolSettings, IDebugAdapterServerProgressManager progressManager) : base(requestRouter, resolverContext, debugAdapterProtocolSettings)
        {
            ProgressManager = progressManager;
        }

        public IDebugAdapterServerProgressManager ProgressManager { get; }
    }
}

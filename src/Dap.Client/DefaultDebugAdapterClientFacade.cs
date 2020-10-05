using DryIoc;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Client;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Client
{
    class DefaultDebugAdapterClientFacade : DebugAdapterProtocolProxy, IDebugAdapterClientFacade
    {
        public DefaultDebugAdapterClientFacade(IResponseRouter requestRouter, IResolverContext resolverContext, IDebugAdapterProtocolSettings debugAdapterProtocolSettings, IDebugAdapterClientProgressManager progressManager) : base(requestRouter, resolverContext, debugAdapterProtocolSettings)
        {
            ProgressManager = progressManager;
        }

        public IDebugAdapterClientProgressManager ProgressManager { get; }
    }
}

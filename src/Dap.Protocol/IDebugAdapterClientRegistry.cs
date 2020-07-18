using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    public interface IDebugAdapterClientRegistry : IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>
    {
    }
}

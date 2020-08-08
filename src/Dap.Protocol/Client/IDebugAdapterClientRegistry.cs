using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Client
{
    public interface IDebugAdapterClientRegistry : IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>
    {
    }
}

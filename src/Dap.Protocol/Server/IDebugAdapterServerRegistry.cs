using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Server
{
    public interface IDebugAdapterServerRegistry : IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>
    {
    }
}

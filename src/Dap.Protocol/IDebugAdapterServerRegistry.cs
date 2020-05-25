using System;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    public interface IDebugAdapterServerRegistry : IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>
    {
    }
}

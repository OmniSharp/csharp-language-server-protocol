using System;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    public interface IDebugAdapterProtocolProxy : IResponseRouter, IDebugAdapterProtocolSettings, IServiceProvider
    {
    }
}

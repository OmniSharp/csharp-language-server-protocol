using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Server
{
    public interface IDebugAdapterServer : IDebugAdapterServerProxy, IJsonRpcHandlerInstance<IDebugAdapterServerRegistry>, IDisposable
    {
        Task Initialize(CancellationToken token);
        IDebugAdapterServerProgressManager ProgressManager { get; }
    }
}

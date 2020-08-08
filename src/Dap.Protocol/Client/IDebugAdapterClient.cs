using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Client
{
    public interface IDebugAdapterClient : IDebugAdapterClientProxy, IJsonRpcHandlerInstance<IDebugAdapterClientRegistry>, IDisposable
    {
        Task Initialize(CancellationToken token);
        IDebugAdapterClientProgressManager ProgressManager { get; }
    }
}

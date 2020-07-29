using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.DebugAdapter.Server;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    public interface IDebugAdapterServer : IResponseRouter, IDisposable
    {
        Task Initialize(CancellationToken token);
        IServerProgressManager ProgressManager { get; }
    }
}

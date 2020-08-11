using System;
using System.Threading;
using System.Threading.Tasks;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Server
{
    public interface IDebugAdapterServer : IDebugAdapterServerProxy, IDisposable
    {
        Task Initialize(CancellationToken token);
        IDebugAdapterServerProgressManager ProgressManager { get; }
    }
}

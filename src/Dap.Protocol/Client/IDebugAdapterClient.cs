using System;
using System.Threading;
using System.Threading.Tasks;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Client
{
    public interface IDebugAdapterClient : IDebugAdapterClientProxy, IDisposable
    {
        Task Initialize(CancellationToken token);
        IDebugAdapterClientProgressManager ProgressManager { get; }
    }
}

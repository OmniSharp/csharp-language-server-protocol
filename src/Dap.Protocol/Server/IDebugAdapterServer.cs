using System;
using System.Threading;
using System.Threading.Tasks;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Server
{
    public interface IDebugAdapterServer : IDebugAdapterServerFacade, IDisposable
    {
        Task Initialize(CancellationToken token);
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IOutputHandler : IDisposable
    {
        void Start();
        void Send(object value, CancellationToken cancellationToken);
        Task WaitForShutdown();
    }
}

using System.Threading.Tasks;
using Lsp.Protocol;

namespace Lsp.Handlers
{
    public class ShutdownHandler : IShutdownHandler
    {
        public Task Handle()
        {
            ShutdownRequested = true;
            Shutdown?.Invoke(ShutdownRequested);
            return Task.CompletedTask;
        }

        public event ShutdownEventHandler Shutdown;

        public bool ShutdownRequested { get; private set; }
    }
}
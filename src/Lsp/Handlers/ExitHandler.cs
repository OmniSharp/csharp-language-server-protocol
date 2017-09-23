using System.Threading.Tasks;
using Lsp.Protocol;

namespace OmniSharp.Extensions.LanguageServerProtocol.Handlers
{
    public class ExitHandler : IExitHandler
    {
        private readonly ShutdownHandler _shutdownHandler;

        public ExitHandler(ShutdownHandler shutdownHandler)
        {
            _shutdownHandler = shutdownHandler;
        }

        public Task Handle()
        {
            Exit?.Invoke(_shutdownHandler.ShutdownRequested ? 0 : 1);
            return Task.CompletedTask;
        }

        public event ExitEventHandler Exit;
    }
}
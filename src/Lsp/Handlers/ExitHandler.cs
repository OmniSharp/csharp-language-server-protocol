using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace OmniSharp.Extensions.LanguageServer.Handlers
{
    public class ExitHandler : IExitHandler
    {
        private readonly ShutdownHandler _shutdownHandler;

        public ExitHandler(ShutdownHandler shutdownHandler)
        {
            _shutdownHandler = shutdownHandler;
        }

        public string Key => nameof(IExitHandler);

        public Task Handle()
        {
            Exit?.Invoke(_shutdownHandler.ShutdownRequested ? 0 : 1);
            return Task.CompletedTask;
        }

        public event ExitEventHandler Exit;
    }
}

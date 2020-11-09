using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Shared
{
    /// <summary>
    /// Ensures that the output handler gets initialized on server started
    /// </summary>
    internal class OutputHandlerInitialized : IOnLanguageServerStarted, IOnLanguageClientStarted
    {
        private readonly OutputHandler _outputHandler;

        public OutputHandlerInitialized(OutputHandler outputHandler)
        {
            _outputHandler = outputHandler;
        }

        public Task OnStarted(ILanguageServer server, CancellationToken cancellationToken)
        {
            _outputHandler.Initialized();
            return Task.CompletedTask;
        }

        public Task OnStarted(ILanguageClient client, CancellationToken cancellationToken)
        {
            _outputHandler.Initialized();
            return Task.CompletedTask;
        }
    }
}

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Server.Abstractions;

namespace OmniSharp.Extensions.LanguageServer.Server.Handlers
{
    public class ExitHandler : IExitHandler
    {
        private readonly ShutdownHandler _shutdownHandler;

        public ExitHandler(ShutdownHandler shutdownHandler)
        {
            _shutdownHandler = shutdownHandler;
        }

        private readonly TaskCompletionSource<int> _exitedSource = new TaskCompletionSource<int>(TaskContinuationOptions.LongRunning);
        public Task WaitForExit => _exitedSource.Task;


        public Task Handle(EmptyRequest request, CancellationToken token)
        {
            var result = _shutdownHandler.ShutdownRequested ? 0 : 1;
            Exit?.Invoke(result);
            _exitedSource.SetResult(result);
            return Task.CompletedTask;
        }

        public event ExitEventHandler Exit;
    }
}

using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Server.Abstractions;

namespace OmniSharp.Extensions.LanguageServer.Server.Handlers
{
    public class ShutdownHandler : IShutdownHandler, IAwaitableTermination
    {
        public event ShutdownEventHandler Shutdown;

        public bool ShutdownRequested { get; private set; }

        private readonly TaskCompletionSource<bool> _shutdownSource = new TaskCompletionSource<bool>(TaskContinuationOptions.LongRunning);
        Task IAwaitableTermination.WasShutDown => _shutdownSource.Task;
        public Task Handle(object request, CancellationToken token)
        {
            ShutdownRequested = true;
            Shutdown?.Invoke(ShutdownRequested);
            _shutdownSource.SetResult(true); // after all event sinks were notified
            return Task.CompletedTask;
        }
    }
}

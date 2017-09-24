using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Abstractions;

namespace OmniSharp.Extensions.LanguageServer.Handlers
{
    public class ShutdownHandler : IShutdownHandler, IAwaitableTermination
    {
        public Task Handle()
        {
            ShutdownRequested = true;
            Shutdown?.Invoke(ShutdownRequested);
            shutdownSource.SetResult(true); // after all event sinks were notified
            return Task.CompletedTask;
        }

        public event ShutdownEventHandler Shutdown;

        public bool ShutdownRequested { get; private set; }

        private readonly TaskCompletionSource<bool> shutdownSource = new TaskCompletionSource<bool>(TaskContinuationOptions.LongRunning);
        Task IAwaitableTermination.WasShutDown => shutdownSource.Task;
    }
}

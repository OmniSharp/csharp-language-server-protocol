using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public partial class LanguageServer : IExitHandler, IShutdownHandler
    {
        private readonly ISubject<int> _exitSubject = new AsyncSubject<int>();
        private readonly ISubject<bool> _shutdownSubject = new AsyncSubject<bool>();
        private bool _shutdownRequested;

        public IObservable<bool> Shutdown => _shutdownSubject.AsObservable();
        public IObservable<int> Exit => _exitSubject.AsObservable();
        public Task WasShutDown => _shutdownSubject.ToTask();
        public Task WaitForExit => _exitSubject.ToTask();

        #pragma warning disable VSTHRD100
        public async void ForcefulShutdown()
        {
            await ( (IShutdownHandler) this ).Handle(ShutdownParams.Instance, CancellationToken.None);
            await ( (IExitHandler) this ).Handle(ExitParams.Instance, CancellationToken.None);
        }

        async Task<Unit> IRequestHandler<ExitParams, Unit>.Handle(ExitParams request, CancellationToken token)
        {
            await Task.Yield();

            var result = _shutdownRequested ? 0 : 1;
            _exitSubject.OnNext(result);
            _exitSubject.OnCompleted();
            await _connection.StopAsync().ConfigureAwait(false);
            return Unit.Value;
        }

        async Task<Unit> IRequestHandler<ShutdownParams, Unit>.Handle(ShutdownParams request, CancellationToken token)
        {
            await Task.Yield(); // Ensure shutdown handler runs asynchronously.

            _shutdownRequested = true;

            try
            {
                _shutdownSubject.OnNext(_shutdownRequested);
            }
            finally
            {
                _shutdownSubject.OnCompleted();
            }

            return Unit.Value;
        }
    }
}

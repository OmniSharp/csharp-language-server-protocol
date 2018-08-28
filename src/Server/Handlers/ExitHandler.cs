using System;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.Embedded.MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Server.Abstractions;

namespace OmniSharp.Extensions.LanguageServer.Server.Handlers
{
    public class ExitHandler : IExitHandler
    {
        private readonly ISubject<int> _exitSubject;
        private readonly ShutdownHandler _shutdownHandler;

        public ExitHandler(ShutdownHandler shutdownHandler)
        {
            _shutdownHandler = shutdownHandler;
            Exit = _exitSubject = new AsyncSubject<int>();
        }

        public Task WaitForExit => Exit.ToTask();
        public IObservable<int> Exit { get; }


        public async Task<Unit> Handle(EmptyRequest request, CancellationToken token)
        {
            await Task.Yield();

            var result = _shutdownHandler.ShutdownRequested ? 0 : 1;
            _exitSubject.OnNext(result);
            _exitSubject.OnCompleted();
            return Unit.Value;
        }
    }
}

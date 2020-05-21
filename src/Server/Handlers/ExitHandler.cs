using System;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Server.Handlers
{
    public class ServerExitHandler : IExitHandler
    {
        private readonly ISubject<int> _exitSubject;
        private readonly ServerShutdownHandler _shutdownHandler;

        public ServerExitHandler(ServerShutdownHandler shutdownHandler)
        {
            _shutdownHandler = shutdownHandler;
            Exit = _exitSubject = new AsyncSubject<int>();
        }

        public Task WaitForExit => Exit.ToTask();
        public IObservable<int> Exit { get; }


        public async Task<Unit> Handle(ExitParams request, CancellationToken token)
        {
            await Task.Yield();

            var result = _shutdownHandler.ShutdownRequested ? 0 : 1;
            _exitSubject.OnNext(result);
            _exitSubject.OnCompleted();
            return Unit.Value;
        }
    }
}

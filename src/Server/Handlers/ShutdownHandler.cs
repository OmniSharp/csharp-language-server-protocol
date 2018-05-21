using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Server.Abstractions;

namespace OmniSharp.Extensions.LanguageServer.Server.Handlers
{
    public class ShutdownHandler : IShutdownHandler
    {
        private readonly ISubject<bool> _shutdownSubject;

        public ShutdownHandler()
        {
            Shutdown = _shutdownSubject = new AsyncSubject<bool>();
        }

        public IObservable<bool> Shutdown { get; }
        public bool ShutdownRequested { get; private set; }
        public Task WasShutDown => Shutdown.ToTask();

        public async Task Handle(EmptyRequest request, CancellationToken token)
        {
            await Task.Yield(); // Ensure shutdown handler runs asynchronously.

            ShutdownRequested = true;
            try
            {
                _shutdownSubject.OnNext(ShutdownRequested);
            }
            finally
            {
                _shutdownSubject.OnCompleted();
            }
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.DebugAdapter.Protocol.Events;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Server
{
    public class ServerProgressManager : ICancelHandler, IServerProgressManager
    {
        private readonly IResponseRouter _router;
        private readonly ISerializer _serializer;

        private readonly ConcurrentDictionary<ProgressToken, CancellationTokenSource> _activeObserverTokens
            = new ConcurrentDictionary<ProgressToken, CancellationTokenSource>(EqualityComparer<ProgressToken>.Default);

        private readonly ConcurrentDictionary<ProgressToken, IProgressObserver> _activeObservers =
            new ConcurrentDictionary<ProgressToken, IProgressObserver>(EqualityComparer<ProgressToken>.Default);

        public ServerProgressManager(IResponseRouter router, ISerializer serializer)
        {
            _router = router;
            _serializer = serializer;
        }

        public IProgressObserver Create(ProgressStartEvent begin, Func<Exception, ProgressEndEvent> onError = null, Func<ProgressEndEvent> onComplete = null)
        {
            if (EqualityComparer<ProgressToken>.Default.Equals(begin.ProgressId, default))
            {
                begin.ProgressId = new ProgressToken(Guid.NewGuid().ToString());
            }

            if (_activeObservers.TryGetValue(begin.ProgressId, out var item))
            {
                return item;
            }

            onError ??= error => new ProgressEndEvent() {
                Message = error.ToString()
            };

            onComplete ??= () => new ProgressEndEvent();

            var cts = new CancellationTokenSource();
            var observer = new ProgressObserver(
                _router,
                begin,
                onError,
                onComplete,
                cts.Token
            );
            _activeObservers.TryAdd(observer.ProgressId, observer);
            _activeObserverTokens.TryAdd(observer.ProgressId, cts);

            return observer;
        }

        public Task<CancelResponse> Handle(CancelArguments request, CancellationToken cancellationToken)
        {
            if (request.ProgressId.HasValue && _activeObserverTokens.TryGetValue(request.ProgressId.Value, out var cts))
            {
                cts.Cancel();
            }

            return Task.FromResult(new CancelResponse());
        }
    }
}

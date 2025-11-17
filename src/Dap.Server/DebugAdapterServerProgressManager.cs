using System.Collections.Concurrent;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Events;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.DebugAdapter.Protocol.Server;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Server
{
    public class DebugAdapterServerProgressManager : ICancelHandler, IDebugAdapterServerProgressManager
    {
        private readonly IResponseRouter _router;

        private readonly ConcurrentDictionary<ProgressToken, CancellationTokenSource> _activeObserverTokens
            = new ConcurrentDictionary<ProgressToken, CancellationTokenSource>(EqualityComparer<ProgressToken>.Default);

        private readonly ConcurrentDictionary<ProgressToken, IProgressObserver> _activeObservers =
            new ConcurrentDictionary<ProgressToken, IProgressObserver>(EqualityComparer<ProgressToken>.Default);

        public DebugAdapterServerProgressManager(IResponseRouter router)
        {
            _router = router;
        }

        public IProgressObserver Create(ProgressStartEvent begin, Func<Exception, ProgressEndEvent>? onError = null, Func<ProgressEndEvent>? onComplete = null)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (begin.ProgressId is null)
            {
                begin = begin with { ProgressId = new ProgressToken(Guid.NewGuid().ToString()) };
            }

            if (_activeObservers.TryGetValue(begin.ProgressId, out var item))
            {
                return item;
            }

            onError ??= error => new ProgressEndEvent {
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
            if (request.ProgressId is not null && _activeObserverTokens.TryGetValue(request.ProgressId, out var cts))
            {
                cts.Cancel();
            }

            return Task.FromResult(new CancelResponse());
        }
    }
}

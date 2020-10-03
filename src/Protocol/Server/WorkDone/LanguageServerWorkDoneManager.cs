using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using ISerializer = OmniSharp.Extensions.LanguageServer.Protocol.Serialization.ISerializer;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.WorkDone
{
    internal class LanguageServerWorkDoneManager : IServerWorkDoneManager
    {
        private readonly IResponseRouter _router;
        private readonly ISerializer _serializer;

        private readonly ConcurrentDictionary<ProgressToken, CancellationTokenSource> _activeObserverTokens
            = new ConcurrentDictionary<ProgressToken, CancellationTokenSource>(EqualityComparer<ProgressToken>.Default);

        private readonly ConcurrentDictionary<ProgressToken, IWorkDoneObserver> _activeObservers
            = new ConcurrentDictionary<ProgressToken, IWorkDoneObserver>(EqualityComparer<ProgressToken>.Default);

        public LanguageServerWorkDoneManager(IResponseRouter router, ISerializer serializer)
        {
            _router = router;
            _serializer = serializer;
        }

        public void Initialized(WindowClientCapabilities windowClientCapabilities) =>
            IsSupported = windowClientCapabilities.WorkDoneProgress.IsSupported;

        public bool IsSupported { get; private set; }

        /// <summary>
        /// Creates a <see cref="IObserver{WorkDoneProgressReport}" /> that will send all of its progress information to the same source.
        /// The other side can cancel this, so the <see cref="CancellationToken" /> should be respected.
        /// </summary>
        public async Task<IWorkDoneObserver> Create(
            ProgressToken progressToken, WorkDoneProgressBegin begin,
            Func<Exception, WorkDoneProgressEnd>? onError = null, Func<WorkDoneProgressEnd>? onComplete = null, CancellationToken cancellationToken = default
        )
        {
            if (!IsSupported)
            {
                return NoopWorkDoneObserver.Instance;
            }

            if (_activeObservers.TryGetValue(progressToken, out var item))
            {
                return item;
            }

            await _router.SendRequest(new WorkDoneProgressCreateParams { Token = progressToken }, cancellationToken).ConfigureAwait(false);

            onError ??= error => new WorkDoneProgressEnd {
                Message = error.ToString()
            };

            onComplete ??= () => new WorkDoneProgressEnd();

            var cts = new CancellationTokenSource();
            var observer = new WorkDoneObserver(
                progressToken,
                _router,
                _serializer,
                begin,
                onError,
                onComplete,
                cts.Token
            );
            _activeObservers.TryAdd(observer.WorkDoneToken, observer);
            _activeObserverTokens.TryAdd(observer.WorkDoneToken, cts);

            return observer;
        }

        /// <summary>
        /// Creates a <see cref="IObserver{WorkDoneProgressReport}" /> that will send all of its progress information to the same source.
        /// The other side can cancel this, so the <see cref="CancellationToken" /> should be respected.
        /// </summary>
        public Task<IWorkDoneObserver> Create(
            WorkDoneProgressBegin begin,
            Func<Exception, WorkDoneProgressEnd>? onError = null, Func<WorkDoneProgressEnd>? onComplete = null, CancellationToken cancellationToken = default
        ) =>
            Create(new ProgressToken(Guid.NewGuid().ToString()), begin, onError, onComplete, cancellationToken);

        /// <summary>
        /// Creates a <see cref="IWorkDoneObserver" /> that will send all of its progress information to the same source.
        /// </summary>
        public IWorkDoneObserver For(
            IWorkDoneProgressParams request,
            WorkDoneProgressBegin begin, Func<Exception, WorkDoneProgressEnd>? onError = null,
            Func<WorkDoneProgressEnd>? onComplete = null
        )
        {
            if (!IsSupported || request.WorkDoneToken == null)
            {
                return NoopWorkDoneObserver.Instance;
            }

            if (_activeObservers.TryGetValue(request.WorkDoneToken, out var item))
            {
                return item;
            }

            onError ??= error => new WorkDoneProgressEnd {
                Message = error.ToString()
            };

            onComplete ??= () => new WorkDoneProgressEnd();
            var cts = new CancellationTokenSource();
            var observer = new WorkDoneObserver(
                request.WorkDoneToken,
                _router,
                _serializer,
                begin,
                onError,
                onComplete,
                cts.Token
            );
            _activeObservers.TryAdd(observer.WorkDoneToken, observer);
            _activeObserverTokens.TryAdd(observer.WorkDoneToken, cts);

            return observer;
        }

        Task<Unit> IRequestHandler<WorkDoneProgressCancelParams, Unit>.Handle(
            WorkDoneProgressCancelParams request, CancellationToken cancellationToken
        )
        {
            if (request.Token == null) return Unit.Task;

            if (_activeObserverTokens.TryRemove(request.Token, out var cts))
            {
                cts.Cancel();
            }

            _activeObservers.TryRemove(request.Token, out _);

            return Unit.Task;
        }
    }
}

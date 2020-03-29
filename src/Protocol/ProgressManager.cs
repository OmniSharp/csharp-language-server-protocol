using System;
using System.Collections.Concurrent;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using ISerializer = OmniSharp.Extensions.LanguageServer.Protocol.Serialization.ISerializer;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public class ProgressManager : IWorkDoneProgressCancelHandler, IProgressHandler
    {
        private IResponseRouter _router;
        private ISerializer _serializer;
        private bool _initialized;
        private bool _supported;
        private readonly ConcurrentDictionary<ProgressToken, (object observer, CancellationTokenSource cts)> _activeObservers
            = new ConcurrentDictionary<ProgressToken, (object observer, CancellationTokenSource cts)>();
        private readonly ConcurrentDictionary<ProgressToken, ISubject<JToken>> _activeObservables
            = new ConcurrentDictionary<ProgressToken, ISubject<JToken>>();

        public void Initialized(IResponseRouter router, ISerializer serializer, WindowClientCapabilities windowClientCapabilities)
        {
            _router = router;
            _serializer = serializer;
            _initialized = true;
            _supported = windowClientCapabilities.WorkDoneProgress.IsSupported &&
                         windowClientCapabilities.WorkDoneProgress.Value;
        }

        public bool IsInitialized => _initialized;

        public bool IsSupported => _supported;

        /// <summary>
        /// Creates a <see cref="IObserver{WorkDoneProgressReport}" /> that will send all of its progress information to the same source.
        /// The other side can cancel this, so the <see cref="CancellationToken" /> should be respected.
        /// </summary>
        public ProgressObserver<WorkDoneProgressReport> Create(WorkDoneProgressBegin begin, Func<Exception, WorkDoneProgressEnd> onError = null, Func<WorkDoneProgressEnd> onComplete = null)
        {
            if (!_initialized || !_supported)
            {
                return ProgressObserver<WorkDoneProgressReport>.Noop;
            }

            onError ??= error => new WorkDoneProgressEnd() {
                Message = error.ToString()
            };

            onComplete ??= () => new WorkDoneProgressEnd();

            var observer = ProgressObserver.CreateWorkDoneProgress(_router, _serializer, begin, onError, onComplete);

            var source = new CancellationTokenSource();
            source.Token.Register(() => {
                if (_activeObservers.TryRemove(observer.Token, out var item))
                    observer.OnCompleted();
            });
            _activeObservers.TryAdd(observer.Token, (observer, source));

            return observer;
        }

        /// <summary>
        /// Creates a <see cref="ProgressObserver" /> that will send all of its progress information to the same source.
        /// </summary>
        public ProgressObserver<WorkDoneProgressReport> WorkDone(IWorkDoneProgressParams request, WorkDoneProgressBegin begin, Func<Exception, WorkDoneProgressEnd> onError = null, Func<WorkDoneProgressEnd> onComplete = null, CancellationToken? cancellationToken = null)
        {
            if (!_initialized || !_supported || request.WorkDoneToken == null)
            {
                return ProgressObserver<WorkDoneProgressReport>.Noop;
            }

            if (_activeObservers.TryGetValue(request.WorkDoneToken, out var item))
            {
                return (ProgressObserver<WorkDoneProgressReport>)item.observer;
            }

            onError ??= error => new WorkDoneProgressEnd() {
                Message = error.ToString()
            };

            onComplete ??= () => new WorkDoneProgressEnd();

            var observer = ProgressObserver.CreateWorkDoneProgress(request.WorkDoneToken, _router, _serializer, begin, onError, onComplete);

            var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken ?? CancellationToken.None);
            source.Token.Register(() => {
                if (_activeObservers.TryRemove(request.WorkDoneToken, out _))
                    observer.OnCompleted();
            });

            _activeObservers.TryAdd(request.WorkDoneToken, (observer, source));

            return observer;
        }

        /// <summary>
        /// Creates a <see cref="IObserver{WorkDoneProgressReport}" /> that will send all of its progress information to the same source.
        /// The other side can cancel this, so the <see cref="CancellationToken" /> should be respected.
        /// </summary>
        public ProgressObserver<T> For<T>(ProgressToken token, CancellationToken? cancellationToken = null)
        {
            if (!_initialized || !_supported || token == null)
            {
                return ProgressObserver<T>.Noop;
            }

            if (_activeObservers.TryGetValue(token, out var item))
            {
                return (ProgressObserver<T>)item.observer;
            }

            var observer = ProgressObserver.Create<T>(token, _router, _serializer);

            var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken ?? CancellationToken.None);
            source.Token.Register(() => {
                if (_activeObservers.TryRemove(token, out var _))
                    observer.OnCompleted();
            });

            _activeObservers.TryAdd(token, (observer, source));

            return observer;
        }

        public ProgressObserver<Container<T>> For<T>(IPartialResultParams<T> request, CancellationToken? cancellationToken = null)
        {
            // This can be null.
            // If you use partial results then your final result must be empty as per the spec
            if (request.PartialResultToken == null) return null;
            return For<Container<T>>(request.PartialResultToken, cancellationToken);
        }

        public ProgressObserver<Container<T>> For<T>(IPartialItems<T> request, CancellationToken? cancellationToken = null)
        {
            // This can be null.
            // If you use partial results then your final result must be empty as per the spec
            if (request.PartialResultToken == null) return null;
            return For<Container<T>>(request.PartialResultToken, cancellationToken);
        }

        public ProgressObserver<T> For<T>(IPartialItem<T> request, CancellationToken? cancellationToken = null)
        {
            // This can be null.
            // If you use partial results then your final result must be empty as per the spec
            if (request.PartialResultToken == null) return null;
            return For<T>(request.PartialResultToken, cancellationToken);
        }

        Task<MediatR.Unit> IRequestHandler<WorkDoneProgressCancelParams, MediatR.Unit>.Handle(WorkDoneProgressCancelParams request, CancellationToken cancellationToken)
        {
            if (_activeObservers.TryGetValue(request.Token, out var item))
            {
                item.cts.Cancel();
            }
            return MediatR.Unit.Task;
        }

        Task<MediatR.Unit> IRequestHandler<ProgressParams, MediatR.Unit>.Handle(ProgressParams request, CancellationToken cancellationToken)
        {
            if (this._activeObservables.TryGetValue(request.Token, out var subject))
            {
                subject.OnNext(request.Value);
            }
            return MediatR.Unit.Task;
        }
    }
}

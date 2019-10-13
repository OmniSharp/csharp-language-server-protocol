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
        private readonly ConcurrentDictionary<ProgressToken, (object observer, CancellationTokenSource cts)> _activeObservers
            = new ConcurrentDictionary<ProgressToken, (object observer, CancellationTokenSource cts)>();
        private readonly ConcurrentDictionary<ProgressToken, ISubject<JObject>> _activeObservables
            = new ConcurrentDictionary<ProgressToken, ISubject<JObject>>();

        public void Initialized(IResponseRouter router, ISerializer serializer)
        {
            _router = router;
            _serializer = serializer;
            _initialized = true;
        }

        public bool IsInitialized => _initialized;

        /// <summary>
        /// Creates a <see cref="IObserver{WorkDoneProgressReport}" /> that will send all of its progress information to the same source.
        /// The other side can cancel this, so the <see cref="CancellationToken" /> should be respected.
        /// </summary>
        public async Task<(ResultObserver<WorkDoneProgressReport>, CancellationToken token)> Create(WorkDoneProgressBegin begin, Func<Exception, WorkDoneProgressEnd> onError = null, Func<WorkDoneProgressEnd> onComplete = null)
        {
            if (!_initialized)
            {
                return (ResultObserver<WorkDoneProgressReport>.Noop, CancellationToken.None);
            }

            ProgressToken token = Guid.NewGuid().ToString();
            await _router.SendRequest(WindowNames.WorkDoneProgressCreate, new WorkDoneProgressCreateParams()
            {
                Token = token,
            });

            onError ??= error =>
            {
                return new WorkDoneProgressEnd()
                {
                    Message = error.ToString()
                };
            };

            onComplete ??= () => null;

            _router.SendProgress(token.Create(token, _serializer.JsonSerializer));

            var observer = new ResultObserver<WorkDoneProgressReport>(Observer.Create<WorkDoneProgressReport>(
                value => _router.SendProgress(token.Create(value, _serializer.JsonSerializer)),
                error => _router.SendProgress(token.Create(onError(error), _serializer.JsonSerializer)),
                () =>
                {
                    if (_activeObservers.TryRemove(token, out var item))
                    {
                        var result = onComplete();
                        if (result == null) return;
                        _router.SendProgress(token.Create(result, _serializer.JsonSerializer));
                    }
                }));

            var source = new CancellationTokenSource();
            source.Token.Register(() =>
            {
                if (_activeObservers.TryRemove(token, out var item))
                    observer.OnCompleted();
            });
            _activeObservers.TryAdd(token, (observer, source));

            return (observer, source.Token);
        }

        /// <summary>
        /// Creates a <see cref="IObserver{WorkDoneProgressReport}" /> that will send all of its progress information to the same source.
        /// </summary>
        public WorkDoneProgressReporter Delegate(IWorkDoneProgressParams request, CancellationToken cancellationToken)
        {
            return new WorkDoneProgressReporter(this, request.WorkDoneToken, cancellationToken);
        }

        /// <summary>
        /// Creates a <see cref="ResultObserver{WorkDoneProgressReport}" /> that will send all of its progress information to the same source.
        /// </summary>
        public ResultObserver<WorkDoneProgressReport> Create(IWorkDoneProgressParams request, WorkDoneProgressBegin begin, Func<WorkDoneProgressEnd> end, CancellationToken cancellationToken)
        {
            if (!_initialized)
            {
                return ResultObserver<WorkDoneProgressReport>.Noop;
            }

            if (request.WorkDoneToken == null)
            {
                return ResultObserver<WorkDoneProgressReport>.Noop;
            }
            return Create(request.WorkDoneToken, begin, end, cancellationToken);
        }

        /// <summary>
        /// Creates a <see cref="ResultObserver{WorkDoneProgressReport}" /> that will send all of its progress information to the same source.
        /// </summary>
        public ResultObserver<WorkDoneProgressReport> Create(ProgressToken token, WorkDoneProgressBegin begin, Func<WorkDoneProgressEnd> end, CancellationToken cancellationToken)
        {
            if (!_initialized)
            {
                return ResultObserver<WorkDoneProgressReport>.Noop;
            }

            if (token == null)
            {
                return ResultObserver<WorkDoneProgressReport>.Noop;
            }

            if (_activeObservers.TryGetValue(token, out var item))
            {
                return (ResultObserver<WorkDoneProgressReport>)item.observer;
            }

            _router.SendProgress(token.Create(begin, _serializer.JsonSerializer));

            var observer = new ResultObserver<WorkDoneProgressReport>(Observer.Create<WorkDoneProgressReport>(
                value => _router.SendProgress(token.Create(value, _serializer.JsonSerializer)),
                () => _router.SendProgress(token.Create(end() ?? new WorkDoneProgressEnd(), _serializer.JsonSerializer)))
            );

            var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            source.Token.Register(() =>
            {
                if (_activeObservers.TryRemove(token, out var _))
                    observer.OnCompleted();
            });

            _activeObservers.TryAdd(token, (observer, source));

            return observer;
        }

        /// <summary>
        /// Creates a <see cref="IObserver{WorkDoneProgressReport}" /> that will send all of its progress information to the same source.
        /// The other side can cancel this, so the <see cref="CancellationToken" /> should be respected.
        /// </summary>
        public ResultObserver<T> For<T>(ProgressToken token, CancellationToken cancellationToken)
        {
            if (!_initialized)
            {
                return ResultObserver<T>.Noop;
            }

            if (token == null)
            {
                return ResultObserver<T>.Noop;
            }

            if (_activeObservers.TryGetValue(token, out var item))
            {
                return (ResultObserver<T>)item.observer;
            }

            var observer = new ResultObserver<T>(Observer.Create<T>(value => _router.SendProgress(token.Create(value, _serializer.JsonSerializer))));

            var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            source.Token.Register(() =>
            {
                if (_activeObservers.TryRemove(token, out var _))
                    observer.OnCompleted();
            });

            _activeObservers.TryAdd(token, (observer, source));

            return observer;
        }

        public ResultObserver<Container<T>> For<T>(IPartialResultParams<T> request, CancellationToken cancellationToken)
        {
            return For<Container<T>>(request.PartialResultToken, cancellationToken);
        }

        public ResultObserver<Container<T>> For<T>(IPartialItems<T> request, CancellationToken cancellationToken)
        {
            return For<Container<T>>(request.PartialResultToken, cancellationToken);
        }

        public ResultObserver<T> For<T>(IPartialItem<T> request, CancellationToken cancellationToken)
        {
            return For<T>(request.PartialResultToken, cancellationToken);
        }

        // public IObservable<T> Observe<T>(ProgressToken token, Func<JObject, T> selector)
        // {
        //     var subject = new Subject<JObject>();
        //     _activeObservables.TryAdd(token, subject);
        //     return subject.Select(selector);
        // }

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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using ISerializer = OmniSharp.Extensions.LanguageServer.Protocol.Serialization.ISerializer;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Progress
{
    public class ProgressManager : IProgressHandler, IProgressManager
    {
        private readonly IResponseRouter _router;
        private readonly ISerializer _serializer;
        private readonly ConcurrentDictionary<ProgressToken, IProgressObserver> _activeObservers = new ConcurrentDictionary<ProgressToken, IProgressObserver>(EqualityComparer<ProgressToken>.Default);
        private readonly ConcurrentDictionary<ProgressToken, IProgressObservable> _activeObservables = new ConcurrentDictionary<ProgressToken, IProgressObservable>(EqualityComparer<ProgressToken>.Default);

        public ProgressManager(IResponseRouter router, ISerializer serializer)
        {
            _router = router;
            _serializer = serializer;
        }

        Task<Unit> IRequestHandler<ProgressParams, Unit>.Handle(ProgressParams request, CancellationToken cancellationToken)
        {
            if (_activeObservables.TryGetValue(request.Token, out var observable) && observable is IObserver<JToken> observer)
            {
                observer.OnNext(request.Value);
            }

            // TODO: Add log message for unhandled?
            return Unit.Task;
        }

        public IProgressObservable<T> Monitor<T>(ProgressToken token)
        {
            return Monitor(token, x => x.ToObject<T>(_serializer.JsonSerializer));
        }

        public IProgressObservable<T> Monitor<T>(ProgressToken token, Func<JToken, T> factory)
        {
            if (_activeObservables.TryGetValue(token, out var o) && o is IProgressObservable<T> observable)
            {
                return observable;
            }

            observable = new ProgressObservable<T>(token, factory, () => _activeObservables.TryRemove(token, out var disposable));
            _activeObservables.TryAdd(token, observable);
            return observable;
        }

        public IRequestProgressObservable<TItem, TResult> MonitorUntil<TItem, TResult>(IPartialItemRequest<TResult, TItem> request, Func<TItem, TResult> factory,
            CancellationToken cancellationToken)
        {
            request.PartialResultToken ??= new ProgressToken(Guid.NewGuid().ToString());
            if (request.PartialResultToken != null && _activeObservables.TryGetValue(request.PartialResultToken, out var o) && o is IRequestProgressObservable<TItem, TResult> observable)
            {
                return observable;
            }

            observable = new RequestProgressObservable<TItem, TResult>(
                _serializer,
                request.PartialResultToken,
                Observable.FromAsync(ct => _router.SendRequest(request, ct)),
                (x, f) => factory(x),
                cancellationToken, () => _activeObservables.TryRemove(request.PartialResultToken, out var disposable));
            _activeObservables.TryAdd(request.PartialResultToken, observable);
            return observable;
        }

        public IRequestProgressObservable<TItem, TResult> MonitorUntil<TItem, TResult>(IPartialItemRequest<TResult, TItem> request, Func<TItem, TResult, TResult> factory,
            CancellationToken cancellationToken)
        {
            request.PartialResultToken ??= new ProgressToken(Guid.NewGuid().ToString());
            if (_activeObservables.TryGetValue(request.PartialResultToken, out var o) && o is IRequestProgressObservable<TItem, TResult> observable)
            {
                return observable;
            }

            observable = new RequestProgressObservable<TItem, TResult>(_serializer, request.PartialResultToken, Observable.FromAsync(ct => _router.SendRequest(request, ct)), factory,
                cancellationToken, () => _activeObservables.TryRemove(request.PartialResultToken, out var disposable));
            _activeObservables.TryAdd(request.PartialResultToken, observable);
            return observable;
        }

        public IRequestProgressObservable<IEnumerable<TItem>, IEnumerable<TItem>> MonitorUntil<TItem>(IPartialItemsRequest<IEnumerable<TItem>, TItem> request,
            CancellationToken cancellationToken)
        {
            request.PartialResultToken ??= new ProgressToken(Guid.NewGuid().ToString());
            if (_activeObservables.TryGetValue(request.PartialResultToken, out var o) && o is IRequestProgressObservable<IEnumerable<TItem>, IEnumerable<TItem>> observable)
            {
                return observable;
            }

            observable = new PartialItemsRequestProgressObservable<TItem, IEnumerable<TItem>>(_serializer,request.PartialResultToken, Observable.FromAsync(ct => _router.SendRequest(request, ct)),
                x => x, cancellationToken, () => _activeObservables.TryRemove(request.PartialResultToken, out var disposable));
            _activeObservables.TryAdd(request.PartialResultToken, observable);
            return observable;
        }

        public IRequestProgressObservable<IEnumerable<TItem>, TResponse> MonitorUntil<TResponse, TItem>(IPartialItemsRequest<TResponse, TItem> request,
            Func<IEnumerable<TItem>, TResponse> factory, CancellationToken cancellationToken)
            where TResponse : IEnumerable<TItem>
        {
            request.PartialResultToken ??= new ProgressToken(Guid.NewGuid().ToString());
            if (_activeObservables.TryGetValue(request.PartialResultToken, out var o) && o is IRequestProgressObservable<IEnumerable<TItem>, TResponse> observable)
            {
                return observable;
            }

            observable = new PartialItemsRequestProgressObservable<TItem, TResponse>(_serializer,request.PartialResultToken, Observable.FromAsync(ct => _router.SendRequest(request, ct)), factory, cancellationToken,
                () => _activeObservables.TryRemove(request.PartialResultToken, out var disposable));
            _activeObservables.TryAdd(request.PartialResultToken, observable);
            return observable;
        }

        public IRequestProgressObservable<TItem> MonitorUntil<TItem>(IPartialItemsRequest<TItem> request, CancellationToken cancellationToken)
        {
            request.PartialResultToken ??= new ProgressToken(Guid.NewGuid().ToString());
            if (_activeObservables.TryGetValue(request.PartialResultToken, out var o) && o is IRequestProgressObservable<TItem> observable)
            {
                return observable;
            }

            observable = new PartialItemsRequestProgressObservable<TItem>(_serializer,request.PartialResultToken, Observable.FromAsync(ct => _router.SendRequest(request, ct)), x => new Container<TItem>(x), cancellationToken,
                () => _activeObservables.TryRemove(request.PartialResultToken, out var disposable));
            _activeObservables.TryAdd(request.PartialResultToken, observable);
            return observable;
        }

        public IProgressObserver<T> For<T>(ProgressToken token, CancellationToken cancellationToken)
        {
            if (_activeObservers.TryGetValue(token, out var o) && o is IProgressObserver<T> observer)
            {
                return observer;
            }

            observer = new ProgressObserver<T>(token, _router, _serializer, cancellationToken, () => _activeObservers.TryRemove(token, out var disposable));
            _activeObservers.TryAdd(token, observer);
            return observer;
        }

        public IProgressObserver<TItem> For<TResponse, TItem>(IPartialItemRequest<TResponse, TItem> request, CancellationToken cancellationToken)
        {
            if (request.PartialResultToken == null) return ProgressObserver<TItem>.Noop;
            if (_activeObservers.TryGetValue(request.PartialResultToken, out var o) && o is IProgressObserver<TItem> observer)
            {
                return observer;
            }

            observer = new ProgressObserver<TItem>(request.PartialResultToken, _router, _serializer, cancellationToken, () => _activeObservers.TryRemove(request.PartialResultToken, out var disposable));
            _activeObservers.TryAdd(request.PartialResultToken, observer);
            return observer;
        }

        public IProgressObserver<IEnumerable<TItem>> For<TResponse, TItem>(IPartialItemsRequest<TResponse, TItem> request, CancellationToken cancellationToken)
            where TResponse : IEnumerable<TItem>
        {
            if (request.PartialResultToken == null) return ProgressObserver<IEnumerable<TItem>>.Noop;
            if (_activeObservers.TryGetValue(request.PartialResultToken, out var o) && o is IProgressObserver<IEnumerable<TItem>> observer)
            {
                return observer;
            }

            observer = new ProgressObserver<IEnumerable<TItem>>(request.PartialResultToken, _router, _serializer, cancellationToken, () => _activeObservers.TryRemove(request.PartialResultToken, out var disposable));
            _activeObservers.TryAdd(request.PartialResultToken, observer);
            return observer;
        }
    }
}

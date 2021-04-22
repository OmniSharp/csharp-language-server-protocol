using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Progress
{
    internal class PartialItemsRequestProgressObservable<TItem, TResult> : IRequestProgressObservable<IEnumerable<TItem>, TResult>, IObserver<JToken>, IDisposable
        where TResult : IEnumerable<TItem>?
    {
        private readonly ISerializer _serializer;
        private readonly ReplaySubject<IEnumerable<TItem>> _dataSubject;
        private readonly CompositeDisposable _disposable;
        private readonly Task<TResult> _task;
        private bool _receivedPartialData;

        public PartialItemsRequestProgressObservable(
            ISerializer serializer,
            ProgressToken token,
            IObservable<TResult> requestResult,
            Func<IEnumerable<TItem>, TResult> factory,
            CancellationToken cancellationToken,
            Action onCompleteAction
        )
        {
            _serializer = serializer;
            _dataSubject = new ReplaySubject<IEnumerable<TItem>>(int.MaxValue);
            _disposable = new CompositeDisposable() { _dataSubject };

            _task = Observable.Create<TResult>(
                                   observer => new CompositeDisposable() {
                                       _dataSubject
                                          .Aggregate(
                                               new List<TItem>(),
                                               (acc, data) => {
                                                   acc.AddRange(data);
                                                   return acc;
                                               }
                                           )
                                          .Select(factory)
                                          .ForkJoin(
                                               requestResult
                                                  .Do(
                                                       result => _dataSubject.OnNext(result ?? Enumerable.Empty<TItem>()),
                                                       _dataSubject.OnError,
                                                       _dataSubject.OnCompleted
                                                   ),
                                               (items, result) => items?.Count() > result?.Count() ? items : result
                                           )
                                          .Subscribe(observer),
                                       Disposable.Create(onCompleteAction)
                                   }
                               )
                              .ToTask(cancellationToken);

            ProgressToken = token;
        }

        public ProgressToken ProgressToken { get; }
        public Type ParamsType { get; } = typeof(TItem);

        void IObserver<JToken>.OnCompleted() => OnCompleted();
        void IObserver<JToken>.OnError(Exception error) => OnError(error);

        private void OnCompleted()
        {
            if (_dataSubject.IsDisposed) return;
            _dataSubject.OnCompleted();
        }

        private void OnError(Exception error)
        {
            if (_dataSubject.IsDisposed) return;
            _dataSubject.OnError(error);
        }

        public void OnNext(JToken value)
        {
            if (_dataSubject.IsDisposed) return;
            _receivedPartialData = true;
            _dataSubject.OnNext(value.ToObject<TItem[]>(_serializer.JsonSerializer));
        }

        public void Dispose()
        {
            if (_disposable.IsDisposed) return;
            _disposable.Dispose();
        }

//        public IDisposable Subscribe(IObserver<IEnumerable<TItem>> observer) => _disposable.IsDisposed ? Disposable.Empty : _dataSubject.Subscribe(observer);
        public IDisposable Subscribe(IObserver<IEnumerable<TItem>> observer) => _dataSubject.Subscribe(observer);

#pragma warning disable VSTHRD003
        public Task<TResult> AsTask() => _task;
#pragma warning restore VSTHRD003
        public TaskAwaiter<TResult> GetAwaiter() => _task.GetAwaiter();
    }

    internal class PartialItemsRequestProgressObservable<TItem> : PartialItemsRequestProgressObservable<TItem, Container<TItem>?>, IRequestProgressObservable<TItem>
    {
        public PartialItemsRequestProgressObservable(
            ISerializer serializer,
            ProgressToken token,
            IObservable<Container<TItem>?> requestResult,
            Func<IEnumerable<TItem>, Container<TItem>?> factory,
            CancellationToken cancellationToken,
            Action onCompleteAction,
            ILogger logger
        ) : base(
            serializer,
            token,
            requestResult,
            factory,
            cancellationToken,
            onCompleteAction
        )
        {
        }
    }
}

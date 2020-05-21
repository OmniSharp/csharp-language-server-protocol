using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Progress
{
    class PartialItemsRequestProgressObservable<TItem, TResult> : IRequestProgressObservable<IEnumerable<TItem>, TResult>, IObserver<JToken>, IDisposable
        where TResult : IEnumerable<TItem>
    {
        private readonly ISerializer _serializer;
        private readonly ISubject<IEnumerable<TItem>> _dataSubject;
        private readonly CompositeDisposable _disposable;
        private readonly Task<TResult> _task;

        public PartialItemsRequestProgressObservable(
            ISerializer serializer,
            ProgressToken token,
            IObservable<TResult> requestResult,
            Func<IEnumerable<TItem>, TResult> factory,
            CancellationToken cancellationToken,
            Action disposal)
        {
            _serializer = serializer;
            _dataSubject = new ReplaySubject<IEnumerable<TItem>>(int.MaxValue);
            var request = requestResult.Do(_ => { }, OnError, OnCompleted).Replay(1);
            _disposable = new CompositeDisposable() {request.Connect(), Disposable.Create(disposal)};

            _task = request.Amb(
                _dataSubject.Scan(new List<TItem>(), (acc, data) => {
                    acc.AddRange(data);
                    return acc;
                }).Select(factory)
            ).ToTask(cancellationToken);
            _task.ContinueWith(x => Dispose());

            ProgressToken = token;
            if (_dataSubject is IDisposable disposable)
            {
                _disposable.Add(disposable);
            }
        }

        public ProgressToken ProgressToken { get; }
        public Type ParamsType { get; } = typeof(TItem);

        public void OnCompleted() => _dataSubject.OnCompleted();

        public void OnError(Exception error) => _dataSubject.OnError(error);

        public void OnNext(JToken value) => _dataSubject.OnNext(value.ToObject<TItem[]>(_serializer.JsonSerializer));

        public void Dispose()
        {
            _disposable.Dispose();
        }

        public IDisposable Subscribe(IObserver<IEnumerable<TItem>> observer)
        {
            return _disposable.IsDisposed ? Disposable.Empty : _dataSubject.Subscribe(observer);
        }

        public Task<TResult> AsTask() => _task;
        public TaskAwaiter<TResult> GetAwaiter() => _task.GetAwaiter();
    }

    class PartialItemsRequestProgressObservable<TItem> : PartialItemsRequestProgressObservable<TItem, Container<TItem>>, IRequestProgressObservable<TItem>
    {
        public PartialItemsRequestProgressObservable(ISerializer serializer, ProgressToken token, IObservable<Container<TItem>> requestResult,
            Func<IEnumerable<TItem>, Container<TItem>> factory, CancellationToken cancellationToken, Action disposal) : base(serializer, token, requestResult, factory, cancellationToken,
            disposal)
        {
        }
    }
}

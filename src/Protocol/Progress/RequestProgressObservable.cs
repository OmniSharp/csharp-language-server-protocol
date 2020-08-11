using System;
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
    internal class RequestProgressObservable<TItem, TResult> : IRequestProgressObservable<TItem, TResult>, IObserver<JToken>
    {
        private readonly ISerializer _serializer;
        private readonly ISubject<TItem> _dataSubject;
        private readonly CompositeDisposable _disposable;
        private readonly Task<TResult> _task;

        public RequestProgressObservable(
            ISerializer serializer,
            ProgressToken token,
            IObservable<TResult> requestResult,
            Func<TItem, TResult, TResult> factory,
            CancellationToken cancellationToken,
            Action disposal
        )
        {
            _serializer = serializer;
            _dataSubject = new ReplaySubject<TItem>(1);
            var request = requestResult.Do(_ => { }, OnError, OnCompleted).Replay(1);
            _disposable = new CompositeDisposable { request.Connect(), Disposable.Create(disposal) };

            _task = _dataSubject.ForkJoin(requestResult, factory).ToTask(cancellationToken);
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

        public void OnNext(JToken value) => _dataSubject.OnNext(value.ToObject<TItem>(_serializer.JsonSerializer));

        public void Dispose() => _disposable.Dispose();

        public IDisposable Subscribe(IObserver<TItem> observer) => _disposable.IsDisposed ? Disposable.Empty : _dataSubject.Subscribe(observer);

        public Task<TResult> AsTask() => _task;
        public TaskAwaiter<TResult> GetAwaiter() => _task.GetAwaiter();
    }
}

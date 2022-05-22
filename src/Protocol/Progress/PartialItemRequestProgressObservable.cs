using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Progress
{
    internal class PartialItemRequestProgressObservable<TItem, TResult> : IRequestProgressObservable<TItem, TResult>, IObserver<JToken>
    {
        private readonly ISerializer _serializer;
        private readonly ReplaySubject<TItem> _dataSubject;
        private readonly CompositeDisposable _disposable;
        private readonly Task<TResult> _task;
        private bool _receivedPartialData;

        public PartialItemRequestProgressObservable(
            ISerializer serializer,
            ProgressToken token,
            IObservable<TResult> requestResult,
            Func<TResult, TItem, TResult> factory,
            Func<TResult, TItem> reverseFactory,
            CancellationToken cancellationToken,
            Action onCompleteAction
        )
        {
            _serializer = serializer;
            _dataSubject = new ReplaySubject<TItem>(1, Scheduler.Immediate);
            _disposable = new CompositeDisposable { _dataSubject };
            _task = Observable.Create<TResult>(
                                   observer => new CompositeDisposable
                                   {
                                       requestResult
                                          .Do(
                                               result =>
                                               {
                                                   if (_receivedPartialData) return;
                                                   _dataSubject.OnNext(reverseFactory(result));
                                               },
                                               _dataSubject.OnError,
                                               _dataSubject.OnCompleted
                                           )
                                          .ForkJoin(_dataSubject, factory)
                                          .Subscribe(observer),
                                       Disposable.Create(onCompleteAction)
                                   }
                               )
                              .ToTask(cancellationToken);

            ProgressToken = token;
        }

        public ProgressToken ProgressToken { get; }
        public Type ParamsType { get; } = typeof(TItem);

        void IObserver<JToken>.OnCompleted()
        {
            OnCompleted();
        }

        void IObserver<JToken>.OnError(Exception error)
        {
            OnError(error);
        }

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
            _dataSubject.OnNext(value.ToObject<TItem>(_serializer.JsonSerializer)!);
        }

        public void Dispose()
        {
            if (_disposable.IsDisposed) return;
            _disposable.Dispose();
        }

        public IDisposable Subscribe(IObserver<TItem> observer)
        {
            return _dataSubject.Subscribe(observer);
        }

#pragma warning disable VSTHRD003
        public Task<TResult> AsTask()
        {
            return _task;
        }
#pragma warning restore VSTHRD003
        public TaskAwaiter<TResult> GetAwaiter()
        {
            return _task.GetAwaiter();
        }
    }
}

using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Progress
{
    internal class ProgressObservable<T> : IProgressObservable<T>, IObserver<JToken>
    {
        private readonly Func<JToken, T> _factory;
        private readonly CompositeDisposable _disposable;
        private readonly ReplaySubject<JToken> _dataSubject;

        public ProgressObservable(ProgressToken token, Func<JToken, T> factory, Action disposal)
        {
            _factory = factory;
            _dataSubject = new ReplaySubject<JToken>(1, Scheduler.Immediate);
            _disposable = new CompositeDisposable { Disposable.Create(_dataSubject.OnCompleted), Disposable.Create(disposal) };

            ProgressToken = token;
            if (_dataSubject is IDisposable disposable)
            {
                _disposable.Add(disposable);
            }
        }

        public ProgressToken ProgressToken { get; }
        public Type ParamsType { get; } = typeof(T);

        void IObserver<JToken>.OnCompleted()
        {
            if (_dataSubject.IsDisposed) return;
            _dataSubject.OnCompleted();
        }

        void IObserver<JToken>.OnError(Exception error)
        {
            if (_dataSubject.IsDisposed) return;
            _dataSubject.OnError(error);
        }

        public void OnNext(JToken value)
        {
            if (_dataSubject.IsDisposed) return;
            _dataSubject.OnNext(value);
        }

        public void Dispose()
        {
            if (_disposable.IsDisposed) return;
            _disposable.Dispose();
        }

        public IDisposable Subscribe(IObserver<T> observer) => _disposable.IsDisposed ? Disposable.Empty : _dataSubject.Select(_factory).Subscribe(observer);
    }
}

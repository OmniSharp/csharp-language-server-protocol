using System;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Events;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;

namespace OmniSharp.Extensions.DebugAdapter.Client
{
    internal class ProgressObservable : IProgressObservable, IObserver<ProgressEvent>, IDisposable
    {
        private readonly CompositeDisposable _disposable;
        private readonly ReplaySubject<ProgressEvent> _dataSubject;

        public ProgressObservable(ProgressToken token)
        {
            _dataSubject = new ReplaySubject<ProgressEvent>(1);
            _disposable = new CompositeDisposable { Disposable.Create(_dataSubject.OnCompleted) };

            ProgressToken = token;
            if (_dataSubject is IDisposable disposable)
            {
                _disposable.Add(disposable);
            }
        }

        public ProgressToken ProgressToken { get; }

        void IObserver<ProgressEvent>.OnCompleted()
        {
            if (_dataSubject.IsDisposed) return;
            _dataSubject.OnCompleted();
        }

        void IObserver<ProgressEvent>.OnError(Exception error)
        {
            if (_dataSubject.IsDisposed) return;
            _dataSubject.OnError(error);
        }

        public void OnNext(ProgressEvent value)
        {
            if (_dataSubject.IsDisposed) return;
            _dataSubject.OnNext(value);
        }

        public void Dispose()
        {
            if (_disposable.IsDisposed) return;
            _disposable.Dispose();
        }

        public IDisposable Subscribe(IObserver<ProgressEvent> observer) => _disposable.IsDisposed ? Disposable.Empty : _dataSubject.Subscribe(observer);
    }
}

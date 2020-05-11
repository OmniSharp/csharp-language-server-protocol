using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public static class ProgressObserver
    {
        public static ProgressObserver<WorkDoneProgressReport> CreateWorkDoneProgress(
            ProgressToken token,
            IResponseRouter router,
            ISerializer serializer,
            WorkDoneProgressBegin begin,
            Func<Exception, WorkDoneProgressEnd> onError,
            Func<WorkDoneProgressEnd> onComplete,
            CancellationToken cancellationToken)
        {
            var disposable = new CompositeDisposable();

            var worker = CreateWorker<WorkDoneProgress>(token, router, serializer, onError, onComplete, disposable);
            worker.OnNext(begin);

            return new ProgressObserver<WorkDoneProgressReport>(token, worker,
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken));
        }

        public static ProgressObserver<WorkDoneProgressReport> CreateWorkDoneProgress(
            IResponseRouter router,
            ISerializer serializer,
            WorkDoneProgressBegin begin, Func<Exception, WorkDoneProgressEnd> onError,
            Func<WorkDoneProgressEnd> onComplete,
            CancellationToken cancellationToken)
        {
            var token = new ProgressToken(Guid.NewGuid().ToString());
            var earlyEvents = new AsyncSubject<List<WorkDoneProgress>>();
            var observer = new Subject<WorkDoneProgress>();
            var disposable = new CompositeDisposable {observer, earlyEvents};

            var worker = CreateWorker<WorkDoneProgress>(token, router, serializer, onError, onComplete, disposable);

            disposable.Add(
                observer
                    .Scan(new List<WorkDoneProgress>() {begin}, (acc, v) =>
                    {
                        acc.Add(v);
                        return acc;
                    })
                    .Subscribe(earlyEvents.OnNext)
            );

            disposable.Add(
                Observable.FromAsync(ct => router.CreateProgress(token, ct))
                    .Subscribe(_ => { }, e => { }, () => { earlyEvents.OnCompleted(); })
            );

            disposable.Add(
                earlyEvents
                    .SelectMany(z => z)
                    .Concat(observer)
                    .Subscribe(worker)
            );

            return new ProgressObserver<WorkDoneProgressReport>(token, observer,
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken));
        }

        public static ProgressObserver<T> Create<T>(
            ProgressToken token, IResponseRouter router,
            ISerializer serializer,
            CancellationToken cancellationToken)
        {
            var observer = new Subject<WorkDoneProgress>();
            var disposable = new CompositeDisposable {observer};

            return new ProgressObserver<T>(token, CreateWorker<T>(token, router, serializer, disposable),
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken));
        }

        private static IObserver<T> CreateWorker<T>(
            ProgressToken token,
            IResponseRouter router,
            ISerializer serializer,
            Func<Exception, WorkDoneProgressEnd> onError,
            Func<WorkDoneProgressEnd> onComplete,
            IDisposable disposable)
        {
            return Observer.Create<T>(
                value => router.SendProgress(token.Create(value)),
                error =>
                {
                    if (onError != null)
                    {
                        router.SendProgress(token.Create(onError.Invoke(error)));
                    }

                    disposable.Dispose();
                },
                () =>
                {
                    var result = onComplete();
                    if (result == null)
                    {
                        disposable.Dispose();
                        return;
                    }

                    router.SendProgress(token.Create(result));
                    disposable.Dispose();
                });
        }

        private static IObserver<T> CreateWorker<T>(
            ProgressToken token,
            IResponseRouter router,
            ISerializer serializer,
            IDisposable disposable)
        {
            return Observer.Create<T>(
                value => router.SendProgress(token.Create(value)),
                error => { disposable.Dispose(); },
                disposable.Dispose);
        }
    }

    public class ProgressObserver<T> : IDisposable, IObserver<T>
    {
        public static ProgressObserver<T> Noop { get; } =
            new ProgressObserver<T>(new ProgressToken(Guid.Empty.ToString()),
                Observer.Create<T>(x => { }), new CancellationTokenSource());

        private readonly IObserver<T> _currentObserver;
        private readonly CancellationTokenSource _tokenSource;

        internal ProgressObserver(ProgressToken progressToken, IObserver<T> observer,
            CancellationTokenSource tokenSource)
        {
            _currentObserver = observer;
            _tokenSource = tokenSource;
            ProgressToken = progressToken;
            CancellationToken = _tokenSource.Token;
        }

        public ProgressToken ProgressToken { get; }
        public CancellationToken CancellationToken { get; }

        public void Dispose()
        {
            if (!_tokenSource.IsCancellationRequested)
            {
                _currentObserver.OnCompleted();
                _tokenSource.Cancel();
            }
        }

        public void OnCompleted()
        {
            if (!_tokenSource.IsCancellationRequested)
            {
                _currentObserver.OnCompleted();
                _tokenSource.Cancel();
            }
        }

        public void OnError(Exception error)
        {
            if (!_tokenSource.IsCancellationRequested)
            {
                _currentObserver.OnError(error);
                _tokenSource.Cancel();
            }
        }

        public void OnNext(T value)
        {
            if (!_tokenSource.IsCancellationRequested)
            {
                _currentObserver.OnNext(value);
            }
        }
    }
}

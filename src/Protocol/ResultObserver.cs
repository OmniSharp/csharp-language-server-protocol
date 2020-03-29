using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
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
            Func<WorkDoneProgressEnd> onComplete)
        {
            var earlyEvents = new AsyncSubject<List<WorkDoneProgress>>();
            var observer = new Subject<WorkDoneProgress>();
            var disposable = new CompositeDisposable {observer, earlyEvents};

            var worker = CreateWorker<WorkDoneProgress>(token, router, serializer, onError, onComplete, disposable);

            disposable.Add(
                observer.Scan(new List<WorkDoneProgress>() {begin}, (acc, v) =>
                    {
                        acc.Add(v);
                        return acc;
                    })
                    .TakeUntil(earlyEvents)
                    .Subscribe(earlyEvents.OnNext)
            );

            disposable.Add(
                Observable.FromAsync(
                    ct => router.SendRequest(WindowNames.WorkDoneProgressCreate,
                        new WorkDoneProgressCreateParams() {Token = token,}, ct)
                ).Subscribe(_ => { }, earlyEvents.OnCompleted)
            );

            disposable.Add(
                earlyEvents.SelectMany(z => z).Concat(observer).Subscribe(worker)
            );

            return new ProgressObserver<WorkDoneProgressReport>(token, observer);
        }

        public static ProgressObserver<WorkDoneProgressReport> CreateWorkDoneProgress(IResponseRouter router, ISerializer serializer,
            WorkDoneProgressBegin begin, Func<Exception, WorkDoneProgressEnd> onError = null,
            Func<WorkDoneProgressEnd> onComplete = null)
        {
            var token = new ProgressToken(Guid.NewGuid().ToString());
            return CreateWorkDoneProgress(token, router, serializer, begin, onError, onComplete);
        }

        public static ProgressObserver<T> Create<T>(
            ProgressToken token, IResponseRouter router,
            ISerializer serializer)
        {
            var observer = new Subject<WorkDoneProgress>();
            var disposable = new CompositeDisposable {observer};

            return new ProgressObserver<T>(token, CreateWorker<T>(token, router, serializer, disposable));
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
                value => router.SendProgress(token.Create(value, serializer.JsonSerializer)),
                error =>
                {
                    router.SendProgress(token.Create(onError(error), serializer.JsonSerializer));
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

                    router.SendProgress(token.Create(result, serializer.JsonSerializer));
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
                value => router.SendProgress(token.Create(value, serializer.JsonSerializer)),
                error => disposable.Dispose(),
                disposable.Dispose);
        }
    }
    public class ProgressObserver<T> : IDisposable, IObserver<T>
    {
        public static ProgressObserver<T> Noop { get; } =
            new ProgressObserver<T>(new ProgressToken(Guid.Empty.ToString()),
                Observer.Create<T>(x => { }));

        private readonly IObserver<T> _currentObserver;

        internal ProgressObserver(ProgressToken token, IObserver<T> observer)
        {
            _currentObserver = observer;
            Token = token;
        }

        public ProgressToken Token { get; }

        public void Dispose()
        {
            _currentObserver.OnCompleted();
        }

        public void OnCompleted()
        {
            _currentObserver.OnCompleted();
        }

        public void OnError(Exception error)
        {
            _currentObserver.OnError(error);
        }

        public void OnNext(T value)
        {
            _currentObserver.OnNext(value);
        }
    }
}

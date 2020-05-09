using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.Extensions.Logging;

namespace OmniSharp.Extensions.JsonRpc
{
    class ProcessScheduler : IDisposable
    {
        private readonly IObserver<(RequestProcessType type, string name, IObservable<Unit> request)> _enqueue;
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        public ProcessScheduler(ILoggerFactory loggerFactory, int? concurrency, IScheduler scheduler)
        {
            var concurrency1 = concurrency;
            var logger = loggerFactory.CreateLogger<ProcessScheduler>();

            var subject = new Subject<(RequestProcessType type, string name, IObservable<Unit> request)>();
            _disposable.Add(subject);
            _enqueue = subject;

            var obs = Observable.Create<Unit>(observer => {
                var cd = new CompositeDisposable();

                var observableQueue =
                    new BehaviorSubject<(RequestProcessType type, ReplaySubject<IObservable<Unit>> observer)>((
                        RequestProcessType.Serial, new ReplaySubject<IObservable<Unit>>(int.MaxValue)));

                cd.Add(subject.Subscribe(item => {
                    if (observableQueue.Value.type != item.type)
                    {
                        observableQueue.Value.observer.OnCompleted();
                        observableQueue.OnNext((item.type, new ReplaySubject<IObservable<Unit>>(int.MaxValue)));
                    }

                    observableQueue.Value.observer.OnNext(HandleRequest(item.name, item.request));
                }));

                cd.Add(observableQueue
                    .Select(item => {
                        var (type, replay) = item;

                        if (type == RequestProcessType.Serial)
                            return replay.Concat();

                        return concurrency1.HasValue
                            ? replay.Merge(concurrency1.Value)
                            : replay.Merge();
                    })
                    .Concat()
                    .Subscribe(observer)
                );

                return cd;
            });

            _disposable.Add(obs
                .ObserveOn(scheduler)
                .Subscribe(_ => { })
            );

            IObservable<Unit> HandleRequest(string name, IObservable<Unit> request)
            {
                return request
                    .Catch<Unit, OperationCanceledException>(ex => Observable.Empty<Unit>())
                    .Catch<Unit, Exception>(ex => {
                        logger.LogCritical(Events.UnhandledException, ex, "Unhandled exception executing {Name}",
                            name);
                        return Observable.Empty<Unit>();
                    });
            }
        }

        public void Add(RequestProcessType type, string name, IObservable<Unit> request)
        {
            _enqueue.OnNext((type, name, request));
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}

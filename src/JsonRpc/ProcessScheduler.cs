using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc.Server;

namespace OmniSharp.Extensions.JsonRpc
{
    class ProcessScheduler : IDisposable
    {
        private readonly IObserver<(RequestProcessType type, string name, SchedulerDelegate request)> _enqueue;
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        public ProcessScheduler(ILoggerFactory loggerFactory, bool supportContentModified, int? concurrency, IScheduler scheduler)
        {
            var concurrency1 = concurrency;
            var logger = loggerFactory.CreateLogger<ProcessScheduler>();

            var subject = new Subject<(RequestProcessType type, string name, SchedulerDelegate request)>();
            _disposable.Add(subject);
            _enqueue = subject;

            var obs = Observable.Create<Unit>(observer => {
                var cd = new CompositeDisposable();

                var observableQueue =
                    new BehaviorSubject<(RequestProcessType type, ReplaySubject<IObservable<Unit>> observer, Subject<Unit> contentModifiedSource)>((
                        RequestProcessType.Serial, new ReplaySubject<IObservable<Unit>>(int.MaxValue), supportContentModified ? new Subject<Unit>() : null));

                cd.Add(subject.Subscribe(item => {
                    if (observableQueue.Value.type != item.type)
                    {
                        if (supportContentModified && observableQueue.Value.type == RequestProcessType.Parallel)
                        {
                            observableQueue.Value.contentModifiedSource.OnCompleted();
                        }
                        observableQueue.Value.observer.OnCompleted();
                        observableQueue.OnNext((item.type, new ReplaySubject<IObservable<Unit>>(int.MaxValue), supportContentModified ? new Subject<Unit>() : null));
                    }

                    observableQueue.Value.observer.OnNext(HandleRequest(item.name, item.request(observableQueue.Value.contentModifiedSource ?? Observable.Never<Unit>())));
                }));

                cd.Add(observableQueue
                    .Select(item => {
                        var (type, replay, _) = item;

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
                    .Catch<Unit, RequestCancelledException>(ex => Observable.Empty<Unit>())
                    .Catch<Unit, ContentModifiedException>(ex => Observable.Empty<Unit>())
                    .Catch<Unit, OperationCanceledException>(ex => Observable.Empty<Unit>())
                    .Catch<Unit, Exception>(ex => {
                        logger.LogCritical(Events.UnhandledException, ex, "Unhandled exception executing {Name}",
                            name);
                        return Observable.Empty<Unit>();
                    });
            }
        }

        public void Add(RequestProcessType type, string name, SchedulerDelegate request)
        {
            _enqueue.OnNext((type, name, request));
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }

    delegate IObservable<Unit> SchedulerDelegate(IObservable<Unit> contentModifiedToken);
}

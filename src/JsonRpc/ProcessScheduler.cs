using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace OmniSharp.Extensions.JsonRpc
{
    public class ProcessScheduler : IScheduler
    {
        private readonly int? _concurrency;
        private readonly ILogger<ProcessScheduler> _logger;
        private readonly IObserver<(RequestProcessType type, string name, IObservable<Unit> request)> _enqueue;
        private readonly IObservable<(RequestProcessType type, string name, IObservable<Unit> request)> _queue;
        private bool _disposed = false;
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        private readonly EventLoopScheduler _scheduler;

        public ProcessScheduler(ILoggerFactory loggerFactory, int? concurrency)
        {
            _concurrency = concurrency;
            _logger = loggerFactory.CreateLogger<ProcessScheduler>();

            var subject = new Subject<(RequestProcessType type, string name, IObservable<Unit> request)>();
            _disposable.Add(subject);
            _enqueue = subject;
            _scheduler = new EventLoopScheduler(
                _ => new Thread(_) {IsBackground = true, Name = "ProcessRequestQueue"});
            _queue = subject
                // .ObserveOn(scheduler)
                // .SubscribeOn(scheduler)
                ;
        }

        public void Start()
        {
            var obs = Observable.Create<Unit>(observer => {
                var cd = new CompositeDisposable();

                RequestProcessType? lastType = null;

                var observableQueue = new BehaviorSubject<(RequestProcessType type, ReplaySubject<IObservable<Unit>> observer)>((
                    RequestProcessType.Serial, new ReplaySubject<IObservable<Unit>>(int.MaxValue)));

                var processor = observableQueue
                    .Select(x => {
                        var (type, observable) = x;

                        if (type == RequestProcessType.Serial)
                            return observable
                                .Do(
                                    start => _logger.LogInformation("serial next {type}", type),
                                    () => _logger.LogInformation("serial complete {type}", type)
                                ).Concat();

                        return _concurrency.HasValue
                            ? observable.Merge(_concurrency.Value)
                            : observable.Merge();
                    })
                    .Concat()
                    .Subscribe(observer);
                cd.Add(processor);

                cd.Add(_queue.Subscribe(item => {
                    var lastItem = observableQueue.Value;
                    if (lastItem.type == item.type)
                    {
                        lastItem.observer.OnNext(
                            item.request.Catch<Unit, OperationCanceledException>(ex => Observable.Empty<Unit>()));
                        return;
                    }

                    lastItem.observer.OnCompleted();

                    var subject = new ReplaySubject<IObservable<Unit>>(int.MaxValue);
                    observableQueue.OnNext((item.type, subject));
                    subject.OnNext(
                        item.request.Catch<Unit, OperationCanceledException>(ex => Observable.Empty<Unit>()));
                }));

                return cd;
            });

            var self = _queue.Do(
                    start => _logger.LogInformation("queue {type}@{name}", start.type, start.name)
                )
                .DistinctUntilChanged(x => x.type)
                .Select(item => {
                    var type = item.type;
                    var observable = _queue
                            .TakeWhile(x => x.type == type)
                            .StartWith(item)
                            .Do(
                                start => _logger.LogInformation("taking request {type}@{name}", start.type, start.name),
                                () => _logger.LogInformation("completed taking request {type}", item.type)
                            )
                            .Select((z, i) => z.request
                                .Do(
                                    start => _logger.LogInformation("before request next {index} {type}@{name}", i,
                                        z.type,
                                        z.name),
                                    () => _logger.LogInformation("before request complete {index} {type}@{name}", i,
                                        z.type,
                                        z.name)
                                )
                                .Catch<Unit, OperationCanceledException>(ex => Observable.Empty<Unit>())
                                .Do(
                                    start => _logger.LogInformation("after request next {index} {type}@{name}", i,
                                        z.type,
                                        z.name),
                                    () => _logger.LogInformation("after request complete {index} {type}@{name}", i,
                                        z.type,
                                        z.name)
                                )
                            )
                            .Do(
                                start => _logger.LogInformation("request done next {type}", item.type),
                                () => _logger.LogInformation("complete {type}", item.type)
                            )
                        ;

                    if (item.type == RequestProcessType.Serial)
                        return observable
                            .Do(
                                start => _logger.LogInformation("serial next {type}", item.type),
                                () => _logger.LogInformation("serial complete {type}", item.type)
                            ).Concat();

                    return _concurrency.HasValue
                        ? observable.Merge(_concurrency.Value)
                        : observable
                            .Merge();
                })
                .Concat()
                .Do(
                    start => _logger.LogInformation("done item"),
                    () => _logger.LogInformation("donedone")
                );

            _disposable.Add(obs
                .ObserveOn(ThreadPoolScheduler.Instance)
                .Subscribe(_ => { })
            );
        }

        public void Add(RequestProcessType type, string name, IObservable<Unit> request)
        {
            _enqueue.OnNext((type, name, request));
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _disposable.Dispose();
        }
    }
}

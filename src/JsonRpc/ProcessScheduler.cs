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
            _queue = subject;
        }

        public void Start()
        {
            var obs = Observable.Create<Unit>(observer => {
                var cd = new CompositeDisposable();

                var observableQueue =
                    new BehaviorSubject<(RequestProcessType type, ReplaySubject<IObservable<Unit>> observer)>((
                        RequestProcessType.Serial, new ReplaySubject<IObservable<Unit>>(int.MaxValue)));

                cd.Add(_queue.Subscribe(item => {
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

                        return _concurrency.HasValue
                            ? replay.Merge(_concurrency.Value)
                            : replay.Merge();
                    })
                    .Concat()
                    .Subscribe(observer)
                );

                return cd;
            });

            _disposable.Add(obs
                .ObserveOn(_scheduler)
                .Subscribe(_ => { })
            );

            IObservable<Unit> HandleRequest(string name, IObservable<Unit> request)
            {
                return request
                    .Catch<Unit, OperationCanceledException>(ex => Observable.Empty<Unit>())
                    .Catch<Unit, Exception>(ex => {
                        _logger.LogCritical(Events.UnhandledException, ex, "Unhandled exception executing {Name}", name);
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
            if (_disposed) return;
            _disposed = true;
            _disposable.Dispose();
        }
    }
}

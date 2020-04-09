using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
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
        private readonly ILogger<ProcessScheduler> _logger;
        private readonly Subject<(RequestProcessType type, string name, IObservable<Unit> request)> _queue;
        private readonly CancellationTokenSource _cancel;

        public ProcessScheduler(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ProcessScheduler>();
            // _queue = new BlockingCollection<(RequestProcessType type, string name, Func<Task> request)>();
            _queue = new Subject<(RequestProcessType type, string name, IObservable<Unit> request)>();
            _cancel = new CancellationTokenSource();
        }

        public void Start()
        {
            var nextSerial = _queue.Where(z => z.type == RequestProcessType.Serial);
            var nextParallel = _queue.Where(z => z.type == RequestProcessType.Parallel);
            var self = _queue
                .DistinctUntilChanged(x => x.type)
                .SelectMany(item => item.type == RequestProcessType.Parallel
                    ? _queue
                        .StartWith(item)
                        .TinakeUntil(nextSerial)
                        .Select(z => z.request
                            .Catch<Unit, OperationCanceledException>(ex => Observable.Empty<Unit>())
                        )
                        .Merge()
                    : _queue
                        .StartWith(item)
                        .TakeUntil(nextParallel)
                        .Select(z => z.request
                            .Catch<Unit, OperationCanceledException>(ex => Observable.Empty<Unit>())
                        )
                        .Concat()
                );

            _subscription = self
                .ObserveOn(new NewThreadScheduler(_ => new Thread(_)
                    {IsBackground = true, Name = "ProcessRequestQueue"}))
                .Subscribe(_ => { });
        }

        public void Add(RequestProcessType type, string name, IObservable<Unit> request)
        {
            _queue.OnNext((type, name, request));
        }

        private bool _disposed = false;
        private IDisposable _subscription;

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _cancel.Cancel();
            _cancel.Dispose();
            _subscription.Dispose();
        }
    }
}

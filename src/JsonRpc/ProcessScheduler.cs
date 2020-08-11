using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc.Server;

namespace OmniSharp.Extensions.JsonRpc
{
    internal class ProcessScheduler : IDisposable
    {
        private readonly IObserver<(RequestProcessType type, string name, SchedulerDelegate request)> _enqueue;
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        public ProcessScheduler(
            ILoggerFactory loggerFactory,
            bool supportContentModified,
            int? concurrency,
            TimeSpan requestTimeout,
            IScheduler scheduler
        )
        {
            var concurrency1 = concurrency;
            var logger = loggerFactory.CreateLogger<ProcessScheduler>();

            var subject = new Subject<(RequestProcessType type, string name, SchedulerDelegate request)>();
            _disposable.Add(subject);
            _enqueue = subject;

            var obs = Observable.Create<Unit>(
                observer => {
                    var cd = new CompositeDisposable();

                    var observableQueue =
                        new BehaviorSubject<(RequestProcessType type, ReplaySubject<IObservable<Unit>> observer, Subject<Unit> contentModifiedSource)>(
                            (
                                RequestProcessType.Serial, new ReplaySubject<IObservable<Unit>>(int.MaxValue), supportContentModified ? new Subject<Unit>() : null )
                        );

                    cd.Add(
                        subject.Subscribe(
                            item => {
                                if (observableQueue.Value.type != item.type)
                                {
                                    logger.LogDebug("Swapping from {From} to {To}", observableQueue.Value.type, item.type);
                                    if (supportContentModified && observableQueue.Value.type == RequestProcessType.Parallel)
                                    {
                                        logger.LogDebug("Cancelling any outstanding requests (switch from parallel to serial)");
                                        observableQueue.Value.contentModifiedSource.OnNext(Unit.Default);
                                        observableQueue.Value.contentModifiedSource.OnCompleted();
                                    }

                                    logger.LogDebug("Completing existing request process type {Type}", observableQueue.Value.type);
                                    observableQueue.Value.observer.OnCompleted();
                                    observableQueue.OnNext(( item.type, new ReplaySubject<IObservable<Unit>>(int.MaxValue), supportContentModified ? new Subject<Unit>() : null ));
                                }

                                logger.LogDebug("Queueing {Type}:{Name} request for processing", item.type, item.name);
                                observableQueue.Value.observer.OnNext(
                                    HandleRequest(item.name, item.request(observableQueue.Value.contentModifiedSource ?? Observable.Never<Unit>(), scheduler))
                                );
                            }
                        )
                    );

                    cd.Add(
                        observableQueue
                           .Select(
                                item => {
                                    var (type, replay, _) = item;

                                    if (type == RequestProcessType.Serial)
                                    {
                                        // logger.LogDebug("Changing to serial processing");
                                        return replay.Concat();
                                    }

                                    if (concurrency1.HasValue)
                                    {
                                        // logger.LogDebug("Changing to parallel processing with concurrency of {Concurrency}", concurrency1.Value);
                                        return replay.Merge(concurrency1.Value);
                                    }

                                    // logger.LogDebug("Changing to parallel processing with concurrency of {Concurrency}", "Unlimited");
                                    return replay.Merge();
                                }
                            )
                           .Concat()
                           .Subscribe(observer)
                    );

                    return cd;
                }
            );

            _disposable.Add(
                obs
                   .ObserveOn(scheduler)
                   .Subscribe(_ => { })
            );

            IObservable<Unit> HandleRequest(string name, IObservable<Unit> request)
            {
                return request
                      .Catch<Unit, RequestCancelledException>(
                           ex => {
                               logger.LogDebug(ex, "Request {Name} was explicitly cancelled", name);
                               return Observable.Empty<Unit>();
                           }
                       )
                      .Catch<Unit, ContentModifiedException>(
                           ex => {
                               logger.LogDebug(ex, "Request {Name} was cancelled, due to content being modified", name);
                               return Observable.Empty<Unit>();
                           }
                       )
                      .Catch<Unit, OperationCanceledException>(
                           ex => {
                               logger.LogDebug(ex, "Request {Name} was cancelled, due to timeout", name);
                               return Observable.Empty<Unit>();
                           }
                       )
                      .Catch<Unit, Exception>(
                           ex => {
                               logger.LogCritical(Events.UnhandledException, ex, "Unhandled exception executing {Name}", name);
                               return Observable.Empty<Unit>();
                           }
                       )
                    // .Do(v => {
                    //     logger.LogDebug("Request {Name} was processed", name);
                    // }, (ex) => {
                    //     logger.LogCritical(Events.UnhandledException, ex, "Request {Name} encountered and unhandled exception", name);
                    // }, () => {
                    //     logger.LogDebug("Request {Name} was completed", name);
                    // })
                    ;
            }
        }

        public void Add(RequestProcessType type, string name, SchedulerDelegate request) => _enqueue.OnNext(( type, name, request ));

        public void Dispose() => _disposable.Dispose();
    }
}

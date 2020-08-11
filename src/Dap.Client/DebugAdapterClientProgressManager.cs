using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Client;
using OmniSharp.Extensions.DebugAdapter.Protocol.Events;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;

namespace OmniSharp.Extensions.DebugAdapter.Client
{
    internal class DebugAdapterClientProgressManager : IProgressStartHandler, IProgressUpdateHandler, IProgressEndHandler, IDebugAdapterClientProgressManager, IDisposable
    {
        private readonly IObserver<IProgressObservable> _observer;
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        private readonly ConcurrentDictionary<ProgressToken, ProgressObservable> _activeObservables =
            new ConcurrentDictionary<ProgressToken, ProgressObservable>(EqualityComparer<ProgressToken>.Default);

        public DebugAdapterClientProgressManager()
        {
            var subject = new Subject<IProgressObservable>();
            _disposable.Add(subject);
            Progress = subject.AsObservable();
            _observer = subject;
        }

        public IObservable<IProgressObservable> Progress { get; }

        Task<Unit> IRequestHandler<ProgressStartEvent, Unit>.Handle(ProgressStartEvent request, CancellationToken cancellationToken)
        {
            var observable = new ProgressObservable(request.ProgressId);
            _activeObservables.TryAdd(request.ProgressId, observable);
            observable.OnNext(request);
            _observer.OnNext(observable);

            return Unit.Task;
        }

        Task<Unit> IRequestHandler<ProgressUpdateEvent, Unit>.Handle(ProgressUpdateEvent request, CancellationToken cancellationToken)
        {
            if (_activeObservables.TryGetValue(request.ProgressId, out var observable))
            {
                observable.OnNext(request);
            }

            // TODO: Add log message for unhandled?
            return Unit.Task;
        }

        Task<Unit> IRequestHandler<ProgressEndEvent, Unit>.Handle(ProgressEndEvent request, CancellationToken cancellationToken)
        {
            if (_activeObservables.TryGetValue(request.ProgressId, out var observable))
            {
                observable.OnNext(request);
            }

            // TODO: Add log message for unhandled?
            return Unit.Task;
        }

        public void Dispose() => _disposable?.Dispose();
    }
}

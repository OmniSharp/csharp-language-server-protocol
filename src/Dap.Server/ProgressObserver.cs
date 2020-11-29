using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Threading;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Events;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Server
{
    internal class ProgressObserver : IProgressObserver
    {
        private readonly IResponseRouter _router;
        private readonly Func<Exception, ProgressEndEvent>? _onError;
        private readonly Func<ProgressEndEvent>? _onComplete;
        private readonly CompositeDisposable _disposable;

        public ProgressObserver(
            IResponseRouter router,
            ProgressStartEvent begin,
            Func<Exception, ProgressEndEvent>? onError,
            Func<ProgressEndEvent>? onComplete,
            CancellationToken cancellationToken
        )
        {
            ProgressId = begin.ProgressId;
            _router = router;
            _onError = onError;
            _onComplete = onComplete;
            _disposable = new CompositeDisposable { Disposable.Create(OnCompleted) };
            cancellationToken.Register(Dispose);
            _router.SendNotification(begin);
        }

        public void OnCompleted()
        {
            if (_disposable.IsDisposed) return;
            var @event = _onComplete?.Invoke() ?? new ProgressEndEvent { Message = "", ProgressId = ProgressId };
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (@event.ProgressId is null)
            {
                @event = @event with { ProgressId = ProgressId };
            }

            _router.SendNotification(@event);
        }

        void IObserver<ProgressUpdateEvent>.OnError(Exception error)
        {
            if (_disposable.IsDisposed) return;
            var @event = _onError?.Invoke(error) ?? new ProgressEndEvent { Message = error.ToString(), ProgressId = ProgressId };
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (@event.ProgressId is null)
            {
                @event = @event with { ProgressId = ProgressId };
            }

            _router.SendNotification(@event);
        }

        public void OnNext(ProgressUpdateEvent value)
        {
            if (_disposable.IsDisposed) return;
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (value.ProgressId is null)
            {
                value = value with { ProgressId = ProgressId };
            }

            _router.SendNotification(value);
        }

        public ProgressToken ProgressId { get; }

        public void OnNext(string message, double? percentage) =>
            OnNext(
                new ProgressUpdateEvent {
                    ProgressId = ProgressId,
                    Message = message,
                    Percentage = percentage
                }
            );

        public void Dispose() => _disposable.Dispose();
    }
}

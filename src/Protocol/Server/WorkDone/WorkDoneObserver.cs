using System;
using System.Reactive.Disposables;
using System.Threading;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using ISerializer = OmniSharp.Extensions.LanguageServer.Protocol.Serialization.ISerializer;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.WorkDone
{
    internal class WorkDoneObserver : IWorkDoneObserver
    {
        private readonly IResponseRouter _router;
        private readonly ISerializer _serializer;
        private readonly Func<Exception, WorkDoneProgressEnd>? _onError;
        private readonly Func<WorkDoneProgressEnd>? _onComplete;
        private readonly CompositeDisposable _disposable;

        public WorkDoneObserver(
            ProgressToken progressToken,
            IResponseRouter router,
            ISerializer serializer,
            WorkDoneProgressBegin begin,
            Func<Exception, WorkDoneProgressEnd>? onError,
            Func<WorkDoneProgressEnd>? onComplete,
            CancellationToken cancellationToken
        )
        {
            WorkDoneToken = progressToken;
            _router = router;
            _serializer = serializer;
            _onError = onError;
            _onComplete = onComplete;
            _disposable = new CompositeDisposable { Disposable.Create(OnCompleted) };
            cancellationToken.Register(Dispose);
            OnNext(begin);
        }

        public void OnCompleted() => _router.SendNotification(
            WorkDoneToken.Create(_onComplete?.Invoke() ?? new WorkDoneProgressEnd { Message = "" }, _serializer.JsonSerializer)
        );

        void IObserver<WorkDoneProgress>.OnError(Exception error) =>
            _router.SendNotification(
                WorkDoneToken.Create(_onError?.Invoke(error) ?? new WorkDoneProgressEnd { Message = error.ToString() }, _serializer.JsonSerializer)
            );

        public void OnNext(WorkDoneProgress value) => _router.SendNotification(
            WorkDoneToken.Create(value, _serializer.JsonSerializer)
        );

        public ProgressToken WorkDoneToken { get; }

        public void OnNext(string message, int? percentage, bool? cancellable) =>
            OnNext(
                new WorkDoneProgressReport {
                    Cancellable = cancellable ?? false,
                    Message = message,
                    Percentage = percentage
                }
            );

        public void Dispose() => _disposable.Dispose();
    }
}

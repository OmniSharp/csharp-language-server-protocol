using System;
using System.Collections.Concurrent;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.WorkDone
{
    [BuiltIn]
    internal class LanguageClientWorkDoneManager : IClientWorkDoneManager, IWorkDoneProgressCreateHandler
    {
        private readonly IWindowLanguageClient _router;
        private readonly ISerializer _serializer;
        private readonly IProgressManager _progressManager;
        private readonly ConcurrentDictionary<ProgressToken, IProgressObservable<WorkDoneProgress>> _pendingWork;

        public LanguageClientWorkDoneManager(IWindowLanguageClient router, ISerializer serializer, IProgressManager progressManager)
        {
            _router = router;
            _serializer = serializer;
            _progressManager = progressManager;
            _pendingWork = new ConcurrentDictionary<ProgressToken, IProgressObservable<WorkDoneProgress>>();
        }

        public void Initialize(WindowClientCapabilities? windowClientCapabilities) =>
            IsSupported = windowClientCapabilities?.WorkDoneProgress.IsSupported == true;

        public bool IsSupported { get; private set; }

        public IProgressObservable<WorkDoneProgress> Monitor(ProgressToken progressToken)
        {
            if (_pendingWork.TryGetValue(progressToken, out var currentValue))
            {
                return currentValue;
            }

            var data = new WorkDoneObservable(
                _progressManager.Monitor(progressToken, Parse),
                Disposable.Create(() => _router.SendWorkDoneProgressCancel(progressToken))
            );
            _pendingWork.AddOrUpdate(progressToken, _ => data, (_, _) => data);
            data.Subscribe(
                _ => { }, () => {
                    if (_pendingWork.TryRemove(data.ProgressToken, out var item))
                    {
                        item.Dispose();
                    }
                }
            );
            return data;
        }

        Task<Unit> IRequestHandler<WorkDoneProgressCreateParams, Unit>.Handle(WorkDoneProgressCreateParams request, CancellationToken cancellationToken)
        {
            if (request.Token != null) Monitor(request.Token);
            return Unit.Task;
        }

        private WorkDoneProgress Parse(JToken token)
        {
            if (!( token is JObject obj ) || !obj.TryGetValue("kind", out var kind)) throw new NotSupportedException("Unknown work done progress event");
            return kind.Value<string>() switch {
                "begin"  => token.ToObject<WorkDoneProgressBegin>(_serializer.JsonSerializer),
                "end"    => token.ToObject<WorkDoneProgressEnd>(_serializer.JsonSerializer),
                "report" => token.ToObject<WorkDoneProgressReport>(_serializer.JsonSerializer),
                _        => throw new NotSupportedException("Unknown work done progress event")
            };
        }
    }

    internal class WorkDoneObservable : IProgressObservable<WorkDoneProgress>
    {
        private readonly IProgressObservable<WorkDoneProgress> _progressObservable;
        private readonly IDisposable _triggerCancellation;

        public WorkDoneObservable(IProgressObservable<WorkDoneProgress> progressObservable, IDisposable triggerCancellation)
        {
            _progressObservable = progressObservable;
            _triggerCancellation = triggerCancellation;
        }

        public ProgressToken ProgressToken => _progressObservable.ProgressToken;

        public Type ParamsType => _progressObservable.ParamsType;

        public void Dispose()
        {
            _triggerCancellation.Dispose();
            _progressObservable.Dispose();
        }

        public IDisposable Subscribe(IObserver<WorkDoneProgress> observer)
        {
            return _progressObservable.Subscribe(
                _ => {
                    observer.OnNext(_);
                    if (_ is WorkDoneProgressEnd)
                    {
                        observer.OnCompleted();
                    }
                },
                observer.OnError,
                observer.OnCompleted
            );
        }
    }
}

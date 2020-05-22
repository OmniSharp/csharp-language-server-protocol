using System;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using MediatR;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.WorkDone
{
    class ClientWorkDoneManager : IClientWorkDoneManager, IWorkDoneProgressCreateHandler
    {
        private readonly ILanguageClient _router;
        private readonly ISerializer _serializer;
        private readonly IProgressManager _progressManager;
        private bool _supported;
        private readonly ISourceCache<IProgressObservable<WorkDoneProgress>, ProgressToken> _pendingWork;

        public ClientWorkDoneManager(ILanguageClient router, ISerializer serializer, IProgressManager progressManager)
        {
            _router = router;
            _serializer = serializer;
            _progressManager = progressManager;
            _pendingWork = new SourceCache<IProgressObservable<WorkDoneProgress>, ProgressToken>(x => x.ProgressToken);
            PendingWork = _pendingWork.AsObservableCache();
        }

        public void Initialize(WindowClientCapabilities windowClientCapabilities)
        {
            _supported = windowClientCapabilities.WorkDoneProgress.IsSupported &&
                         windowClientCapabilities.WorkDoneProgress.Value;
        }

        public bool IsSupported => _supported;
        public IObservableCache<IProgressObservable<WorkDoneProgress>, ProgressToken> PendingWork { get; }

        public IProgressObservable<WorkDoneProgress> Monitor(ProgressToken progressToken)
        {
            var currentValue = _pendingWork.Lookup(progressToken);
            if (_pendingWork.Lookup(progressToken).HasValue)
            {
                return currentValue.Value;
            }

            var data = new WorkDoneObservable(
                _progressManager.Monitor(progressToken, Parse),
                Disposable.Create(() => _router.Window.SendWorkDoneProgressCancel(progressToken))
            );
            _pendingWork.AddOrUpdate(data);
            data.Subscribe(_ => { }, () => {
                _pendingWork.RemoveKey(data.ProgressToken);
                _pendingWork.Dispose();
            });
            return data;
        }

        Task<Unit> IRequestHandler<WorkDoneProgressCreateParams, Unit>.Handle(WorkDoneProgressCreateParams request, CancellationToken cancellationToken)
        {
            Monitor(request.Token);
            return Unit.Task;
        }

        private WorkDoneProgress Parse(JToken token)
        {
            if (!(token is JObject obj) || !obj.TryGetValue("kind", out var kind)) throw new NotSupportedException("Unknown work done progress event");
            return kind.Value<string>() switch {
                "begin" => token.ToObject<WorkDoneProgressBegin>(_serializer.JsonSerializer),
                "end" => token.ToObject<WorkDoneProgressEnd>(_serializer.JsonSerializer),
                "report" => token.ToObject<WorkDoneProgressReport>(_serializer.JsonSerializer),
                _ => throw new NotSupportedException("Unknown work done progress event")
            };
        }
    }

    class WorkDoneObservable : IProgressObservable<WorkDoneProgress>
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

        public IDisposable Subscribe(IObserver<WorkDoneProgress> observer) => _progressObservable.Subscribe(observer);
    }
}

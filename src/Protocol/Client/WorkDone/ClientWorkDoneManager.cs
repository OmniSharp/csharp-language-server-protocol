using System;
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
        private readonly IResponseRouter _router;
        private readonly ISerializer _serializer;
        private readonly IProgressManager _progressManager;
        private bool _supported;
        private readonly ISourceCache<IProgressObservable<WorkDoneProgress>, ProgressToken> _pendingWork;

        public ClientWorkDoneManager(IResponseRouter router, ISerializer serializer, IProgressManager progressManager)
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
            var data = _progressManager.Monitor(progressToken, Parse);
            _pendingWork.AddOrUpdate(data);
            data.Subscribe(_ => { }, () => { _pendingWork.RemoveKey(data.ProgressToken); });
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
}

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.WorkDone;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{

    public abstract class ServerProxyBase : IServerProxy
    {
        private readonly IResponseRouter _responseRouter;
        private readonly ILanguageProtocolSettings _settings;

        public ServerProxyBase(
            IResponseRouter requestRouter,
            IProgressManager progressManager,
            IServerWorkDoneManager serverWorkDoneManager,
            ILanguageServerConfiguration languageServerConfiguration,
            ILanguageProtocolSettings settings
        )
        {
            _responseRouter = requestRouter;
            ProgressManager = progressManager;
            WorkDoneManager = serverWorkDoneManager;
            Configuration = languageServerConfiguration;
            _settings = settings;
        }

        public void SendNotification(string method) => _responseRouter.SendNotification(method);

        public void SendNotification<T>(string method, T @params) => _responseRouter.SendNotification(method, @params);

        public void SendNotification(IRequest request) => _responseRouter.SendNotification(request);

        public IResponseRouterReturns SendRequest<T>(string method, T @params) => _responseRouter.SendRequest(method, @params);

        public IResponseRouterReturns SendRequest(string method) => _responseRouter.SendRequest(method);

        public Task<TResponse> SendRequest<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken) => _responseRouter.SendRequest(request, cancellationToken);

        (string method, TaskCompletionSource<JToken> pendingTask) IResponseRouter.GetRequest(long id) => _responseRouter.GetRequest(id);
        public IProgressManager ProgressManager { get; }

        public IServerWorkDoneManager WorkDoneManager { get; }

        public ILanguageServerConfiguration Configuration { get; }

        public InitializeParams ClientSettings => _settings.ClientSettings;

        public InitializeResult ServerSettings => _settings.ServerSettings;
    }
}

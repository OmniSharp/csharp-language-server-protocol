using System;
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

    abstract class ServerProxyBase : IServerProxy
    {
        private readonly IResponseRouter _responseRouter;
        private readonly ILanguageProtocolSettings _settings;
        private readonly Lazy<IServerWorkDoneManager> _workDoneManager;
        private readonly Lazy<ILanguageServerConfiguration> _configuration;

        public ServerProxyBase(
            IResponseRouter requestRouter,
            IProgressManager progressManager,
            Lazy<IServerWorkDoneManager> serverWorkDoneManager,
            Lazy<ILanguageServerConfiguration> languageServerConfiguration,
            ILanguageProtocolSettings settings
        )
        {
            _responseRouter = requestRouter;
            ProgressManager = progressManager;
            _workDoneManager = serverWorkDoneManager;
            _configuration = languageServerConfiguration;
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

        public IServerWorkDoneManager WorkDoneManager => _workDoneManager.Value;

        public ILanguageServerConfiguration Configuration => _configuration.Value;

        public InitializeParams ClientSettings => _settings.ClientSettings;

        public InitializeResult ServerSettings => _settings.ServerSettings;
    }
}

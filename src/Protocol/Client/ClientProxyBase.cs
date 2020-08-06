using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.WorkDone;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public abstract class ClientProxyBase : IClientProxy
    {
        private readonly IResponseRouter _responseRouter;
        private readonly ILanguageProtocolSettings _settings;

        public ClientProxyBase(
            IResponseRouter requestRouter,
            IProgressManager progressManager,
            IClientWorkDoneManager clientWorkDoneManager,
            IRegistrationManager registrationManager,
            IWorkspaceFoldersManager workspaceFoldersManager,
            ILanguageProtocolSettings settings
        )
        {
            _responseRouter = requestRouter;
            ProgressManager = progressManager;
            WorkDoneManager = clientWorkDoneManager;
            RegistrationManager = registrationManager;
            WorkspaceFoldersManager = workspaceFoldersManager;
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
        public IClientWorkDoneManager WorkDoneManager { get; }

        public IRegistrationManager RegistrationManager { get; }

        public IWorkspaceFoldersManager WorkspaceFoldersManager { get; }

        public InitializeParams ClientSettings => _settings.ClientSettings;

        public InitializeResult ServerSettings => _settings.ServerSettings;
    }
}

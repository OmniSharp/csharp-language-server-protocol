using System;
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
    internal abstract class ClientProxyBase : IClientProxy
    {
        private readonly IResponseRouter _responseRouter;
        private readonly ILanguageProtocolSettings _settings;

        private readonly Lazy<IClientWorkDoneManager> _workDoneManager;
        private readonly Lazy<IRegistrationManager> _registrationManager;
        private readonly Lazy<IWorkspaceFoldersManager> _workspaceFoldersManager;

        public ClientProxyBase(
            IResponseRouter requestRouter,
            IProgressManager progressManager,
            Lazy<IClientWorkDoneManager> clientWorkDoneManager,
            Lazy<IRegistrationManager> registrationManager,
            Lazy<IWorkspaceFoldersManager> workspaceFoldersManager,
            ILanguageProtocolSettings settings
        )
        {
            _responseRouter = requestRouter;
            ProgressManager = progressManager;
            _workDoneManager = clientWorkDoneManager;
            _registrationManager = registrationManager;
            _workspaceFoldersManager = workspaceFoldersManager;
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

        public IClientWorkDoneManager WorkDoneManager => _workDoneManager.Value;

        public IRegistrationManager RegistrationManager => _registrationManager.Value;

        public IWorkspaceFoldersManager WorkspaceFoldersManager => _workspaceFoldersManager.Value;

        public InitializeParams ClientSettings => _settings.ClientSettings;

        public InitializeResult ServerSettings => _settings.ServerSettings;
    }
}

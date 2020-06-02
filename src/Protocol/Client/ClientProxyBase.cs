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
    public abstract class ClientProxyBase : IClientProxy
    {
        private readonly IClientProxy _proxy;
        private readonly IServiceProvider _serviceProvider;

        public ClientProxyBase(IClientProxy proxy, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _proxy = proxy;
        }

        public void SendNotification(string method) => _proxy.SendNotification(method);

        public void SendNotification<T>(string method, T @params) => _proxy.SendNotification(method, @params);

        public void SendNotification(IRequest request) => _proxy.SendNotification(request);

        public IResponseRouterReturns SendRequest<T>(string method, T @params) => _proxy.SendRequest(method, @params);

        public IResponseRouterReturns SendRequest(string method) => _proxy.SendRequest(method);

        public Task<TResponse> SendRequest<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken) => _proxy.SendRequest(request, cancellationToken);

        (string method, TaskCompletionSource<JToken> pendingTask) IResponseRouter.GetRequest(long id) => _proxy.GetRequest(id);
        object IServiceProvider.GetService(Type serviceType) => _serviceProvider.GetService(serviceType);
        public IProgressManager ProgressManager => _proxy.ProgressManager;
        public IClientWorkDoneManager WorkDoneManager => _proxy.WorkDoneManager;

        public IRegistrationManager RegistrationManager => _proxy.RegistrationManager;

        public IWorkspaceFoldersManager WorkspaceFoldersManager => _proxy.WorkspaceFoldersManager;

        public InitializeParams ClientSettings => _proxy.ClientSettings;

        public InitializeResult ServerSettings => _proxy.ServerSettings;
    }
}

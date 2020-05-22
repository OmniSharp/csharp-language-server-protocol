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
    public abstract class ServerProxyBase : IServerProxy
    {
        private readonly IServerProxy _proxy;
        private readonly IServiceProvider _serviceProvider;

        public ServerProxyBase(IServerProxy proxy, IServiceProvider serviceProvider)
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

        public TaskCompletionSource<JToken> GetRequest(long id) => _proxy.GetRequest(id);
        object IServiceProvider.GetService(Type serviceType) => _serviceProvider.GetService(serviceType);
        public IProgressManager ProgressManager => _proxy.ProgressManager;
        public IServerWorkDoneManager WorkDoneManager => _proxy.WorkDoneManager;
        public ILanguageServerConfiguration Configuration => _proxy.Configuration;

        public InitializeParams ClientSettings => _proxy.ClientSettings;

        public InitializeResult ServerSettings => _proxy.ServerSettings;
    }
}

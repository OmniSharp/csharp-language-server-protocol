using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    internal abstract class LanguageProtocolProxy : ILanguageProtocolProxy
    {
        private readonly IResponseRouter _responseRouter;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILanguageProtocolSettings _languageProtocolSettings;

        public LanguageProtocolProxy(
            IResponseRouter requestRouter, IServiceProvider serviceProvider, IProgressManager progressManager, ILanguageProtocolSettings languageProtocolSettings
        )
        {
            ProgressManager = progressManager;
            _responseRouter = requestRouter;
            _serviceProvider = serviceProvider;
            _languageProtocolSettings = languageProtocolSettings;
        }

        public IProgressManager ProgressManager { get; }
        public InitializeParams ClientSettings => _languageProtocolSettings.ClientSettings;

        public InitializeResult ServerSettings => _languageProtocolSettings.ServerSettings;

        public void SendNotification(string method) => _responseRouter.SendNotification(method);

        public void SendNotification<T>(string method, T @params) => _responseRouter.SendNotification(method, @params);

        public void SendNotification(IRequest request) => _responseRouter.SendNotification(request);

        public IResponseRouterReturns SendRequest<T>(string method, T @params) => _responseRouter.SendRequest(method, @params);

        public IResponseRouterReturns SendRequest(string method) => _responseRouter.SendRequest(method);

        public Task<TResponse> SendRequest<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken) => _responseRouter.SendRequest(request, cancellationToken);

        (string method, TaskCompletionSource<JToken> pendingTask) IResponseRouter.GetRequest(long id) => _responseRouter.GetRequest(id);
        object IServiceProvider.GetService(Type serviceType) => _serviceProvider.GetService(serviceType);
    }
}

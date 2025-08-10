using System.Diagnostics.CodeAnalysis;
using DryIoc;
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
        protected readonly IResolverContext ResolverContext;
        private readonly ILanguageProtocolSettings _languageProtocolSettings;

        public LanguageProtocolProxy(
            IResponseRouter requestRouter, IResolverContext resolverContext, IProgressManager progressManager,
            ILanguageProtocolSettings languageProtocolSettings
        )
        {
            ProgressManager = progressManager;
            _responseRouter = requestRouter;
            ResolverContext = resolverContext;
            _languageProtocolSettings = languageProtocolSettings;
        }

        public IProgressManager ProgressManager { get; }
        public InitializeParams ClientSettings => _languageProtocolSettings.ClientSettings;

        public InitializeResult ServerSettings => _languageProtocolSettings.ServerSettings;

        public void SendNotification(string method)
        {
            _responseRouter.SendNotification(method);
        }

        public void SendNotification<T>(string method, T @params)
        {
            _responseRouter.SendNotification(method, @params);
        }

        public void SendNotification(IRequest<Unit> request)
        {
            _responseRouter.SendNotification(request);
        }

        public IResponseRouterReturns SendRequest<T>(string method, T @params)
        {
            return _responseRouter.SendRequest(method, @params);
        }

        public IResponseRouterReturns SendRequest(string method)
        {
            return _responseRouter.SendRequest(method);
        }

        public Task<TResponse> SendRequest<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken)
        {
            return _responseRouter.SendRequest(request, cancellationToken);
        }

        bool IResponseRouter.TryGetRequest(long id, [NotNullWhen(true)] out string? method, [NotNullWhen(true)] out TaskCompletionSource<JToken>? pendingTask)
        {
            return _responseRouter.TryGetRequest(id, out method, out pendingTask);
        }

        object IServiceProvider.GetService(Type serviceType)
        {
            return ResolverContext.GetService(serviceType);
        }
    }
}

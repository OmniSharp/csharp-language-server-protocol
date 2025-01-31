using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using DryIoc;
using MediatR;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    public interface IDebugAdapterProtocolProxy : IResponseRouter, IDebugAdapterProtocolSettings, IServiceProvider
    {
    }

    internal abstract class DebugAdapterProtocolProxy : IDebugAdapterProtocolProxy
    {
        private readonly IResponseRouter _responseRouter;
        protected readonly IResolverContext ResolverContext;
        private readonly IDebugAdapterProtocolSettings _debugAdapterProtocolSettings;

        public DebugAdapterProtocolProxy(
            IResponseRouter requestRouter, IResolverContext resolverContext, IDebugAdapterProtocolSettings debugAdapterProtocolSettings
        )
        {
            _responseRouter = requestRouter;
            ResolverContext = resolverContext;
            _debugAdapterProtocolSettings = debugAdapterProtocolSettings;
        }

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

        public InitializeRequestArguments ClientSettings => _debugAdapterProtocolSettings.ClientSettings;
        public InitializeResponse ServerSettings => _debugAdapterProtocolSettings.ServerSettings;
    }
}

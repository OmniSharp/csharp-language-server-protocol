using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace OmniSharp.Extensions.JsonRpc
{
    public abstract class JsonRpcServerBase: IResponseRouter
    {
        private readonly IJsonRpcServerOptions _options;

        public JsonRpcServerBase(IJsonRpcServerOptions options)
        {
            _options = options;
        }
        protected abstract IResponseRouter ResponseRouter { get; }
        protected abstract IHandlersManager HandlersManager { get; }

        public void SendNotification(string method)
        {
            ResponseRouter.SendNotification(method);
        }

        public void SendNotification<T>(string method, T @params)
        {
            ResponseRouter.SendNotification(method, @params);
        }

        public void SendNotification(IRequest @params)
        {
            ResponseRouter.SendNotification(@params);
        }

        public Task<TResponse> SendRequest<TResponse>(IRequest<TResponse> @params, CancellationToken cancellationToken)
        {
            return ResponseRouter.SendRequest(@params, cancellationToken);
        }

        public IResponseRouterReturns SendRequest<T>(string method, T @params)
        {
            return ResponseRouter.SendRequest(method, @params);
        }

        public IResponseRouterReturns SendRequest(string method)
        {
            return ResponseRouter.SendRequest(method);
        }

        (string method, TaskCompletionSource<JToken> pendingTask) IResponseRouter.GetRequest(long id)
        {
            return ResponseRouter.GetRequest(id);
        }

        protected void EnsureAllHandlersAreRegistered()
        {
            var foundHandlers = _options.Services
                .Where(x => typeof(IJsonRpcHandler).IsAssignableFrom(x.ServiceType) &&
                            x.ServiceType != typeof(IJsonRpcHandler))
                .ToArray();

            // Handlers are created at the start and maintained as a singleton
            foreach (var handler in foundHandlers)
            {
                _options.Services.Remove(handler);

                if (handler.ImplementationFactory != null)
                    _options.Services.Add(ServiceDescriptor.Singleton(typeof(IJsonRpcHandler), handler.ImplementationFactory));
                else if (handler.ImplementationInstance != null)
                    _options.Services.Add(ServiceDescriptor.Singleton(typeof(IJsonRpcHandler), handler.ImplementationInstance));
                else
                    _options.Services.Add(ServiceDescriptor.Singleton(typeof(IJsonRpcHandler), handler.ImplementationType));
            }

            _options.Services.AddJsonRpcMediatR(_options.Assemblies.Distinct());
        }
    }
}

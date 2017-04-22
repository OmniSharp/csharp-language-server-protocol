using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using JsonRpc.Server;
using JsonRpc.Server.Messages;

namespace JsonRpc
{
    public class IncomingRequestRouter : IIncomingRequestRouter
    {
        private readonly HandlerResolver _resolver;
        private readonly IServiceProvider _serviceProvider;

        public IncomingRequestRouter(HandlerResolver resolver, IServiceProvider serviceProvider)
        {
            _resolver = resolver;
            _serviceProvider = serviceProvider;
        }

        public async void RouteNotification(Notification notification)
        {
            var method = _resolver.GetMethod(notification.Method);

            // Q: Should be report notification not found?
            if (method is null) return;


            var service = _serviceProvider.GetService(method.ServiceInterface);

            // TODO: Try / catch for Internal Error

            Task result;
            if (method.Params is null)
            {
                result = method.HandleNotification(service);
            }
            else
            {
                var @params = notification.Params.ToObject(method.Params);
                result = method.HandleNotification(service, @params);
            }

            await result;
        }

        private string GetId(object id)
        {
            if (id is string s)
            {
                return s;
            }

            if (id is long l)
            {
                return l.ToString();
            }

            return null;
        }

        public Task<ErrorResponse> RouteRequest(Request request)
        {
            return RouteRequest(request, CancellationToken.None);
        }

        protected virtual async Task<ErrorResponse> RouteRequest(Request request, CancellationToken token)
        {
            var method = _resolver.GetMethod(request.Method);
            if (method is null)
            {
                return new MethodNotFound(request.Id);
            }

            var service = _serviceProvider.GetService(method.ServiceInterface);

            Task result;
            if (method.Params is null)
            {
                result = method.HandleRequest(service, token);
            }
            else
            {
                object @params;
                try
                {
                    @params = request.Params.ToObject(method.Params);
                }
                catch
                {
                    return new InvalidParams(request.Id);
                }

                result = method.HandleRequest(service, @params, token);
            }

            await result;

            object responseValue = null;
            if (result.GetType().GetTypeInfo().IsGenericType)
            {
                var property = typeof(Task<>)
                    .MakeGenericType(result.GetType().GetTypeInfo().GetGenericArguments()[0])
                    .GetProperty(nameof(Task<object>.Result), BindingFlags.Public | BindingFlags.Instance);

                responseValue = property.GetValue(result);
            }

            return new Client.Response(request.Id, responseValue);
        }
    }

    public class OutgoingRequestRouter : IOutgoingRequestRouter
    {
        private readonly HandlerResolver _resolver;
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentDictionary<string, CancellationTokenSource> _requests = new ConcurrentDictionary<string, CancellationTokenSource>();

        public OutgoingRequestRouter(HandlerResolver resolver, IServiceProvider serviceProvider)
        {
            _resolver = resolver;
            _serviceProvider = serviceProvider;
        }

        public Task SendNotification<T>(string method, T @params)
        {
            throw new NotImplementedException();
        }

        public Task<TResponse> SendRequest<T, TResponse>(string method, T @params)
        {
            throw new NotImplementedException();
        }

        public Task SendRequest<T>(string method, T @params)
        {
            throw new NotImplementedException();
        }
    }
}
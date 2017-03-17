using System;
using System.Reflection;
using System.Threading.Tasks;
using JsonRpc.Server;
using JsonRpc.Server.Messages;

namespace JsonRpc
{
    public class Mediator : IMediator
    {
        private readonly HandlerResolver _resolver;
        private readonly IServiceProvider _serviceProvider;

        public Mediator(HandlerResolver resolver, IServiceProvider serviceProvider)
        {
            _resolver = resolver;
            _serviceProvider = serviceProvider;
        }

        public async void HandleNotification(Notification notification)
        {
            var method = _resolver.GetMethod(notification.Method);

            // Q: Should be report notification not found?
            if (method is null) return;


            var service = _serviceProvider.GetService(method.ServiceInterface);

            // TODO: Try / catch for Internal Error

            Task result;
            if (method.Params is null)
            {
                result = method.Handle(service);
            }
            else
            {
                var @params = notification.Params.ToObject(method.Params);
                result = method.Handle(service, @params);
            }

            await result;
        }

        public async Task<ErrorResponse> HandleRequest(Request request)
        {
            var method = _resolver.GetMethod(request.Method);
            if (method is null) return new MethodNotFound(request.Id);

            var service = _serviceProvider.GetService(method.ServiceInterface);

            // TODO: Try / catch for Internal Error

            Task result;
            if (method.Params is null)
            {
                result = method.Handle(service);
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

                result = method.Handle(service, @params);
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

            return new Response(request.Id, responseValue);
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
using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using JsonRpc.Server;
using JsonRpc.Server.Messages;

namespace JsonRpc
{
    public class Mediator : IMediator
    {
        private readonly HandlerResolver _resolver;
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentDictionary<string, CancellationTokenSource> _requests = new ConcurrentDictionary<string, CancellationTokenSource>();

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

        public async Task<ErrorResponse> HandleRequest(Request request)
        {
            var method = _resolver.GetMethod(request.Method);
            if (method is null)
            {
                return new MethodNotFound(request.Id);
            }

            var id = GetId(request.Id);
            var cts = new CancellationTokenSource();
            _requests.TryAdd(id, cts);

            Task result;
            // TODO: Try / catch for Internal Error
            try
            {
                var service = _serviceProvider.GetService(method.ServiceInterface);


                if (method.Params is null)
                {
                    result = method.HandleRequest(service, cts.Token);
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

                    result = method.HandleRequest(service, @params, cts.Token);
                }

                await result;

            }
            catch (TaskCanceledException)
            {
                return new RequestCancelled();
            }

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

        public void CancelRequest(object id)
        {
            if (_requests.TryGetValue(GetId(id), out var cts))
            {
                cts.Cancel();
            }
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
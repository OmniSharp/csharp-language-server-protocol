using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.JsonRpc.Server.Messages;

namespace OmniSharp.Extensions.JsonRpc
{
    class RequestRouter : IRequestRouter
    {
        private readonly HandlerCollection _collection;

        public RequestRouter(HandlerCollection collection)
        {
            _collection = collection;
        }

        public IDisposable Add(IJsonRpcHandler handler)
        {
            return _collection.Add(handler);
        }

        private IHandlerDescriptor FindDescriptor(IMethodWithParams instance)
        {
            return _collection.FirstOrDefault(x => x.Method == instance.Method);
        }

        public async Task RouteNotification(IHandlerDescriptor handler, Notification notification)
        {
            Task result;
            if (handler.Params is null)
            {
                result = ReflectionRequestHandlers.HandleNotification(handler);
            }
            else
            {
                var @params = notification.Params.ToObject(handler.Params);
                result = ReflectionRequestHandlers.HandleNotification(handler, @params);
            }
            await result.ConfigureAwait(false);
        }

        public Task<ErrorResponse> RouteRequest(IHandlerDescriptor descriptor, Request request)
        {
            return RouteRequest(descriptor, request, CancellationToken.None);
        }

        protected virtual async Task<ErrorResponse> RouteRequest(IHandlerDescriptor handler, Request request, CancellationToken token)
        {
            if (request.Method is null)
            {
                return new MethodNotFound(request.Id, request.Method);
            }

            Task result;
            if (handler.Params is null)
            {
                result = ReflectionRequestHandlers.HandleRequest(handler, token);
            }
            else
            {
                object @params;
                try
                {
                    @params = request.Params.ToObject(handler.Params);
                }
                catch
                {
                    return new InvalidParams(request.Id);
                }

                result = ReflectionRequestHandlers.HandleRequest(handler, @params, token);
            }

            await result.ConfigureAwait(false);

            object responseValue = null;
            if (result.GetType().GetTypeInfo().IsGenericType)
            {
                var property = typeof(Task<>)
                    .MakeGenericType(result.GetType().GetTypeInfo().GetGenericArguments()[0]).GetTypeInfo()
                    .GetProperty(nameof(Task<object>.Result), BindingFlags.Public | BindingFlags.Instance);

                responseValue = property.GetValue(result);
            }

            return new Client.Response(request.Id, responseValue);
        }

        public IHandlerDescriptor GetDescriptor(Notification notification)
        {
            return FindDescriptor(notification);
        }

        public IHandlerDescriptor GetDescriptor(Request request)
        {
            return FindDescriptor(request);
        }

        Task IRequestRouter.RouteNotification(Notification notification)
        {
            return RouteNotification(GetDescriptor(notification), notification);
        }

        Task<ErrorResponse> IRequestRouter.RouteRequest(Request request)
        {
            return RouteRequest(GetDescriptor(request), request);
        }
    }
}

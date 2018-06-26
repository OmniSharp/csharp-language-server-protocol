using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.JsonRpc.Server.Messages;

namespace OmniSharp.Extensions.JsonRpc
{
    public class RequestRouter : IRequestRouter
    {
        private readonly HandlerCollection _collection;
        private readonly ISerializer _serializer;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RequestRouter(HandlerCollection collection, ISerializer serializer, IServiceScopeFactory serviceScopeFactory)
        {
            _collection = collection;
            _serializer = serializer;
            _serviceScopeFactory = serviceScopeFactory;
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
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IRequestContext>();
                context.Descriptor = handler;

                object @params = null;
                if (!(handler.Params is null))
                {
                    @params = notification.Params.ToObject(handler.Params, _serializer.JsonSerializer);
                }
                await MediatRHandlers.HandleNotification(scope.ServiceProvider.GetRequiredService<IMediator>(), handler, @params ?? EmptyRequest.Instance, CancellationToken.None).ConfigureAwait(false);
            }
        }

        public Task<ErrorResponse> RouteRequest(IHandlerDescriptor descriptor, Request request)
        {
            return RouteRequest(descriptor, request, CancellationToken.None);
        }

        protected virtual async Task<ErrorResponse> RouteRequest(IHandlerDescriptor handler, Request request, CancellationToken token)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IRequestContext>();
                context.Descriptor = handler;

                if (request.Method is null)
                {
                    return new MethodNotFound(request.Id, request.Method);
                }

                object @params;
                try
                {
                    @params = request.Params.ToObject(handler.Params, _serializer.JsonSerializer);
                }
                catch
                {
                    return new InvalidParams(request.Id);
                }

                var result = MediatRHandlers.HandleRequest(scope.ServiceProvider.GetRequiredService<IMediator>(), handler, @params, token);

                await result.ConfigureAwait(false);

                object responseValue = null;
                if (result.GetType().GetTypeInfo().IsGenericType)
                {
                    var property = typeof(Task<>)
                        .MakeGenericType(result.GetType().GetTypeInfo().GetGenericArguments()[0]).GetTypeInfo()
                        .GetProperty(nameof(Task<object>.Result), BindingFlags.Public | BindingFlags.Instance);

                    responseValue = property.GetValue(result);
                    if (responseValue?.GetType() == typeof(Unit))
                    {
                        responseValue = null;
                    }
                }

                return new Client.Response(request.Id, responseValue);
            }
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

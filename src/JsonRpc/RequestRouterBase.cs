using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.JsonRpc.Server.Messages;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using OmniSharp.Extensions.JsonRpc.Client;
using Newtonsoft.Json.Linq;

namespace OmniSharp.Extensions.JsonRpc
{
    public abstract class RequestRouterBase<TDescriptor> : IRequestRouter<TDescriptor>
        where TDescriptor : IHandlerDescriptor
    {
        protected readonly ISerializer _serializer;
        protected readonly IServiceScopeFactory _serviceScopeFactory;
        protected readonly ILogger _logger;

        public RequestRouterBase(ISerializer serializer, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, ILogger logger)
        {
            _serializer = serializer;
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            ServiceProvider = serviceProvider;
        }

        public IServiceProvider ServiceProvider { get; }

        public async Task RouteNotification(IRequestDescriptor<TDescriptor> descriptors, Notification notification, object @params, CancellationToken token)
        {
            using var debug = _logger.TimeDebug("Routing Notification {Method}", notification.Method);
            using var _ = _logger.BeginScope(new[] {
                new KeyValuePair<string, string>("Method", notification.Method),
                new KeyValuePair<string, string>("Params", notification.Params?.ToString())
            });

            await Task.WhenAll(descriptors.Select(descriptor => InnerRoute(_serviceScopeFactory, descriptor, @params, token)));

            static async Task InnerRoute(IServiceScopeFactory serviceScopeFactory, TDescriptor descriptor, object @params, CancellationToken token)
            {
                using var scope = serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<IRequestContext>();
                context.Descriptor = descriptor;
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                if (descriptor.Params is null)
                {
                    await HandleNotification(mediator, descriptor, EmptyRequest.Instance, token);
                }
                else
                {
                    await HandleNotification(mediator, descriptor, @params ?? Activator.CreateInstance(descriptor.Params), token);
                }
            }
        }

        public virtual async Task<ErrorResponse> RouteRequest(IRequestDescriptor<TDescriptor> descriptors, Request request, object @params, CancellationToken token)
        {
            using var debug = _logger.TimeDebug("Routing Request ({Id}) {Method}", request.Id, request.Method);
            using var _ = _logger.BeginScope(new[] {
                new KeyValuePair<string, string>("Id", request.Id?.ToString()),
                new KeyValuePair<string, string>("Method", request.Method),
                new KeyValuePair<string, string>("Params", request.Params?.ToString())
            });

            // TODO: Do we want to support more handlers as "aggregate"?
            if (typeof(IEnumerable<object>).IsAssignableFrom(descriptors.Default.Response))
            {
                var responses = await Task.WhenAll(descriptors.Select(descriptor => InnerRoute(_serviceScopeFactory, request, descriptor, @params, token, _logger)));
                var errorResponse = responses.FirstOrDefault(x => x.IsError);
                if (errorResponse.IsError) return errorResponse;
                if (responses.Length == 1)
                {
                    return responses[0];
                }

                var response = Activator.CreateInstance(
                    typeof(AggregateResponse<>).MakeGenericType(descriptors.Default.Response),
                    new object[] { responses.Select(z => z.Response.Result) }
                );
                return new OutgoingResponse(request.Id, response, request);
            }

            return await InnerRoute(_serviceScopeFactory, request, descriptors.Default, @params, token, _logger);

            static async Task<ErrorResponse> InnerRoute(IServiceScopeFactory serviceScopeFactory, Request request, TDescriptor descriptor, object @params, CancellationToken token,
                ILogger logger)
            {
                using var scope = serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<IRequestContext>();
                context.Descriptor = descriptor;
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                token.ThrowIfCancellationRequested();

                var result = HandleRequest(mediator, descriptor, @params ?? Activator.CreateInstance(descriptor.Params), token);
                await result;

                token.ThrowIfCancellationRequested();

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

                    logger.LogTrace("Response value was {Type}", responseValue?.GetType().FullName);
                }

                return new OutgoingResponse(request.Id, responseValue, request);
            }
        }

        public abstract IRequestDescriptor<TDescriptor> GetDescriptors(Notification notification);
        public abstract IRequestDescriptor<TDescriptor> GetDescriptors(Request request);
        protected virtual object DeserializeParams(IHandlerDescriptor descriptor, JToken @params)
        {
            if (descriptor.IsDelegatingHandler)
            {
                _logger.LogTrace("Converting params for {Method} to {Type}", descriptor.Method, descriptor.Params.GetGenericArguments()[0].FullName);
                var o = @params?.ToObject(descriptor.Params.GetGenericArguments()[0], _serializer.JsonSerializer);
                return Activator.CreateInstance(descriptor.Params, new object[] { o });
            }
            else
            {
                _logger.LogTrace("Converting params for {Method} to {Type}", descriptor.Method, descriptor.Params.FullName);
                return @params?.ToObject(descriptor.Params, _serializer.JsonSerializer);
            }
        }

        private static readonly MethodInfo SendRequestUnit = typeof(RequestRouterBase<TDescriptor>)
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
            .Where(x => x.Name == nameof(SendRequest))
            .First(x => x.GetGenericArguments().Length == 1);

        private static readonly MethodInfo SendRequestResponse = typeof(RequestRouterBase<TDescriptor>)
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
            .Where(x => x.Name == nameof(SendRequest))
            .First(x => x.GetGenericArguments().Length == 2);

        public static Task HandleNotification(IMediator mediator, IHandlerDescriptor handler, object @params, CancellationToken token)
        {
            return (Task)SendRequestUnit
                .MakeGenericMethod(handler.Params ?? typeof(EmptyRequest))
                .Invoke(null, new object[] { mediator, @params, token });
        }

        public static Task HandleRequest(IMediator mediator, IHandlerDescriptor descriptor, object @params, CancellationToken token)
        {
            if (!descriptor.HasReturnType)
            {
                return (Task)SendRequestUnit
                    .MakeGenericMethod(descriptor.Params)
                    .Invoke(null, new object[] { mediator, @params, token });
            }
            else
            {
                return (Task)SendRequestResponse
                    .MakeGenericMethod(descriptor.Params, descriptor.Response)
                    .Invoke(null, new object[] { mediator, @params, token });
            }
        }

        private static Task SendRequest<T>(IMediator mediator, T request, CancellationToken token)
            where T : IRequest
        {
            return mediator.Send(request, token);
        }

        private static Task<TResponse> SendRequest<T, TResponse>(IMediator mediator, T request, CancellationToken token)
            where T : IRequest<TResponse>
        {
            return mediator.Send(request, token);
        }
    }
}

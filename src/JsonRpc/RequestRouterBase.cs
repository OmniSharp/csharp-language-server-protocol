using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc.Client;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.JsonRpc.Server.Messages;

namespace OmniSharp.Extensions.JsonRpc
{
    public abstract class RequestRouterBase<TDescriptor> : IRequestRouter<TDescriptor>
        where TDescriptor : IHandlerDescriptor
    {
        protected readonly ISerializer _serializer;
        protected readonly IServiceScopeFactory _serviceScopeFactory;
        protected readonly ILogger _logger;

        public RequestRouterBase(ISerializer serializer, IServiceScopeFactory serviceScopeFactory, ILogger logger)
        {
            _serializer = serializer;
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        public async Task RouteNotification(IRequestDescriptor<TDescriptor> descriptors, Notification notification, CancellationToken token)
        {
            using var debug = _logger.TimeDebug("Routing Notification {Method}", notification.Method);
            using var _ = _logger.BeginScope(
                new[] {
                    new KeyValuePair<string, string?>("Method", notification.Method),
                    new KeyValuePair<string, string?>("Params", notification.Params?.ToString())
                }
            );

            object? @params = null;
            if (!( descriptors.Default?.Params is null ))
            {
                if (descriptors.Default.IsDelegatingHandler)
                {
                    _logger.LogTrace("Converting params for Notification {Method} to {Type}", notification.Method, descriptors.Default.Params.GetGenericArguments()[0].FullName);
                    var o = notification.Params?.ToObject(descriptors.Default.Params.GetGenericArguments()[0], _serializer.JsonSerializer);
                    @params = Activator.CreateInstance(descriptors.Default.Params, o);
                }
                else
                {
                    _logger.LogTrace("Converting params for Notification {Method} to {Type}", notification.Method, descriptors.Default.Params.FullName);
                    @params = notification.Params?.ToObject(descriptors.Default.Params, _serializer.JsonSerializer);
                }
            }

            await Task.WhenAll(descriptors.Select(descriptor => InnerRoute(_serviceScopeFactory, descriptor, @params, token))).ConfigureAwait(false);

            static async Task InnerRoute(IServiceScopeFactory serviceScopeFactory, TDescriptor descriptor, object? @params, CancellationToken token)
            {
                using var scope = serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<IRequestContext>();
                context.Descriptor = descriptor;
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                await HandleNotification(mediator, descriptor, @params ?? Activator.CreateInstance(descriptor.Params), token).ConfigureAwait(false);
            }
        }

        public virtual async Task<ErrorResponse> RouteRequest(IRequestDescriptor<TDescriptor> descriptors, Request request, CancellationToken token)
        {
            Debug.Assert(descriptors.Default != null);
            Debug.Assert(descriptors.Default!.Params != null);
            Debug.Assert(descriptors.Default!.Response != null);
            using var debug = _logger.TimeDebug("Routing Request ({Id}) {Method}", request.Id, request.Method);
            using var _ = _logger.BeginScope(
                new[] {
                    new KeyValuePair<string, string?>("Id", request.Id.ToString()),
                    new KeyValuePair<string, string?>("Method", request.Method),
                    new KeyValuePair<string, string?>("Params", request.Params?.ToString())
                }
            );

            object? @params;
            try
            {
                if (descriptors.Default!.IsDelegatingHandler)
                {
                    _logger.LogTrace(
                        "Converting params for Request ({Id}) {Method} to {Type}", request.Id, request.Method,
                        descriptors.Default!.Params!.GetGenericArguments()[0].FullName
                    );
                    var o = request.Params?.ToObject(descriptors.Default!.Params!.GetGenericArguments()[0], _serializer.JsonSerializer);
                    @params = Activator.CreateInstance(descriptors.Default!.Params, o);
                }
                else
                {
                    _logger.LogTrace("Converting params for Request ({Id}) {Method} to {Type}", request.Id, request.Method, descriptors.Default!.Params!.FullName);
                    _logger.LogTrace("Converting params for Notification {Method} to {Type}", request.Method, descriptors.Default!.Params.FullName);
                    @params = request.Params?.ToObject(descriptors.Default!.Params, _serializer.JsonSerializer);
                }
            }
            catch (Exception cannotDeserializeRequestParams)
            {
                _logger.LogError(new EventId(-32602), cannotDeserializeRequestParams, "Failed to deserialize request parameters.");
                return new InvalidParams(request.Id, request.Method);
            }

            using var scope = _serviceScopeFactory.CreateScope();
            // TODO: Do we want to support more handlers as "aggregate"?
            if (typeof(IEnumerable).IsAssignableFrom(descriptors.Default!.Response) && typeof(string) != descriptors.Default!.Response && !typeof(JToken).IsAssignableFrom(descriptors.Default!.Response))
            {
                var responses = await Task.WhenAll(descriptors.Select(descriptor => InnerRoute(_serviceScopeFactory, request, descriptor, @params, token, _logger))).ConfigureAwait(false);
                var errorResponse = responses.FirstOrDefault(x => x.IsError);
                if (errorResponse.IsError) return errorResponse;
                if (responses.Length == 1)
                {
                    return responses[0];
                }

                var response = Activator.CreateInstance(
                    typeof(AggregateResponse<>).MakeGenericType(descriptors.Default!.Response!), responses.Select(z => z.Response!.Result)
                );
                return new OutgoingResponse(request.Id, response, request);
            }

            return await InnerRoute(_serviceScopeFactory, request, descriptors.Default!, @params, token, _logger).ConfigureAwait(false);

            static async Task<ErrorResponse> InnerRoute(
                IServiceScopeFactory serviceScopeFactory, Request request, TDescriptor descriptor, object? @params, CancellationToken token,
                ILogger logger
            )
            {
                using var scope = serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<IRequestContext>();
                context.Descriptor = descriptor;
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                token.ThrowIfCancellationRequested();

                var result = HandleRequest(mediator, descriptor, @params ?? Activator.CreateInstance(descriptor.Params!), token);
                await result.ConfigureAwait(false);

                token.ThrowIfCancellationRequested();

                object? responseValue = null;
                if (result.GetType().GetTypeInfo().IsGenericType)
                {
                    var property = typeof(Task<>)
                                  .MakeGenericType(result.GetType().GetTypeInfo().GetGenericArguments()[0]).GetTypeInfo()
                                  .GetProperty(nameof(Task<object>.Result), BindingFlags.Public | BindingFlags.Instance)!;

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

        private static readonly MethodInfo SendRequestUnit = typeof(RequestRouterBase<TDescriptor>)
                                                            .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                                                            .Where(x => x.Name == nameof(SendRequest))
                                                            .First(x => x.GetGenericArguments().Length == 1);

        private static readonly MethodInfo SendRequestResponse = typeof(RequestRouterBase<TDescriptor>)
                                                                .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                                                                .Where(x => x.Name == nameof(SendRequest))
                                                                .First(x => x.GetGenericArguments().Length == 2);

        public static Task HandleNotification(IMediator mediator, IHandlerDescriptor handler, object @params, CancellationToken token) =>
            (Task) SendRequestUnit
                  .MakeGenericMethod(handler.Params!)
                  .Invoke(null, new[] { mediator, @params, token });

        public static Task HandleRequest(IMediator mediator, IHandlerDescriptor descriptor, object @params, CancellationToken token)
        {
            if (!descriptor.HasReturnType)
            {
                return (Task) SendRequestUnit
                             .MakeGenericMethod(descriptor.Params!)
                             .Invoke(null, new[] { mediator, @params, token });
            }

            return (Task) SendRequestResponse
                         .MakeGenericMethod(descriptor.Params!, descriptor.Response!)
                         .Invoke(null, new[] { mediator, @params, token });
        }

        private static Task SendRequest<T>(IMediator mediator, T request, CancellationToken token)
            where T : IRequest =>
            mediator.Send(request, token);

        private static Task<TResponse> SendRequest<T, TResponse>(IMediator mediator, T request, CancellationToken token)
            where T : IRequest<TResponse> =>
            mediator.Send(request, token);
    }
}

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

namespace OmniSharp.Extensions.JsonRpc
{
    public abstract class RequestRouterBase<TDescriptor> : IRequestRouter<TDescriptor>
        where TDescriptor : IHandlerDescriptor
    {
        protected readonly ISerializer _serializer;
        protected readonly IServiceScopeFactory _serviceScopeFactory;
        protected readonly ILogger _logger;
        private readonly ConcurrentDictionary<string, CancellationTokenSource> _requests = new ConcurrentDictionary<string, CancellationTokenSource>();


        public RequestRouterBase(ISerializer serializer, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, ILogger logger)
        {
            _serializer = serializer;
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            ServiceProvider = serviceProvider;
        }

        public IServiceProvider ServiceProvider { get; }

        public async Task RouteNotification(TDescriptor descriptor, Notification notification, CancellationToken token)
        {
            using (_logger.TimeDebug("Routing Notification {Method}", notification.Method))
            {
                using (_logger.BeginScope(new[] {
                    new KeyValuePair<string, string>( "Method", notification.Method),
                    new KeyValuePair<string, string>( "Params", notification.Params?.ToString())
                }))
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<IRequestContext>();
                    context.Descriptor = descriptor;
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                    try
                    {
                        if (descriptor.Params is null)
                        {
                            await HandleNotification(mediator, descriptor, EmptyRequest.Instance, token);
                        }
                        else
                        {
                            _logger.LogDebug("Converting params for Notification {Method} to {Type}", notification.Method, descriptor.Params.FullName);
                            object @params;
                            if (descriptor.IsDelegatingHandler)
                            {
                                // new DelegatingRequest();
                                var o = notification.Params?.ToObject(descriptor.Params.GetGenericArguments()[0], _serializer.JsonSerializer);
                                @params = Activator.CreateInstance(descriptor.Params, new object[] { o });
                            }
                            else
                            {
                                @params = notification.Params?.ToObject(descriptor.Params, _serializer.JsonSerializer);
                            }

                            await HandleNotification(mediator, descriptor, @params ?? Activator.CreateInstance(descriptor.Params), token);
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogCritical(Events.UnhandledRequest, e, "Failed to handle request {Method}", notification.Method);
                    }
                }
            }
        }

        public virtual async Task<ErrorResponse> RouteRequest(TDescriptor descriptor, Request request, CancellationToken token)
        {
            using (_logger.TimeDebug("Routing Request ({Id}) {Method}", request.Id, request.Method))
            {
                using (_logger.BeginScope(new[] {
                    new KeyValuePair<string, string>( "Id", request.Id?.ToString()),
                    new KeyValuePair<string, string>( "Method", request.Method),
                    new KeyValuePair<string, string>( "Params", request.Params?.ToString())
                }))
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<IRequestContext>();
                    context.Descriptor = descriptor;
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                    var id = GetId(request.Id);
                    if (!_requests.TryGetValue(id, out var cts))
                    {
                        cts = new CancellationTokenSource();
                        _requests.TryAdd(id, cts);
                    }
                    token.Register(cts.Cancel);

                    // TODO: Try / catch for Internal Error
                    try
                    {
                        if (cts.IsCancellationRequested)
                        {
                            _logger.LogDebug("Request {Id} was cancelled", id);
                            return new RequestCancelled();
                        }

                        // To avoid boxing, the best way to compare generics for equality is with EqualityComparer<T>.Default.
                        // This respects IEquatable<T> (without boxing) as well as object.Equals, and handles all the Nullable<T> "lifted" nuances.
                        // https://stackoverflow.com/a/864860
                        if (EqualityComparer<TDescriptor>.Default.Equals(descriptor, default))
                        {
                            _logger.LogDebug("descriptor not found for Request ({Id}) {Method}", request.Id, request.Method);
                            return new MethodNotFound(request.Id, request.Method);
                        }

                        object @params;
                        try
                        {
                            _logger.LogTrace("Converting params for Request ({Id}) {Method} to {Type}", request.Id, request.Method, descriptor.Params.FullName);
                            if (descriptor.IsDelegatingHandler)
                            {
                                // new DelegatingRequest();
                                var o = request.Params?.ToObject(descriptor.Params.GetGenericArguments()[0], _serializer.JsonSerializer);
                                @params = Activator.CreateInstance(descriptor.Params, new object[] { o });
                            }
                            else
                            {
                                @params = request.Params?.ToObject(descriptor.Params, _serializer.JsonSerializer);
                            }
                        }
                        catch (Exception cannotDeserializeRequestParams)
                        {
                            _logger.LogError(new EventId(-32602), cannotDeserializeRequestParams, "Failed to deserialize request parameters.");
                            return new InvalidParams(request.Id);
                        }

                        var result = HandleRequest(mediator, descriptor, @params ?? EmptyRequest.Instance, cts.Token);
                        await result;

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
                            _logger.LogTrace("Response value was {Type}", responseValue?.GetType().FullName);
                        }

                        return new JsonRpc.Client.Response(request.Id, responseValue, request);
                    }
                    catch (OperationCanceledException)
                    {
                        _logger.LogDebug("Request {Id} was cancelled", id);
                        return new RequestCancelled();
                    }
                    catch (RpcErrorException e)
                    {
                        _logger.LogCritical(Events.UnhandledRequest, e, "Failed to handle notification {Method}", request.Method);
                        return new RpcError(id, new ErrorMessage(e.Code, e.Message, e.Error));
                    }
                    catch (Exception e)
                    {
                        _logger.LogCritical(Events.UnhandledRequest, e, "Failed to handle notification {Method}", request.Method);
                        return new InternalError(id, e.ToString());
                    }
                    finally
                    {
                        _requests.TryRemove(id, out var _);
                    }
                }
            }
        }

        public void CancelRequest(object id)
        {
            if (_requests.TryGetValue(GetId(id), out var cts))
            {
                _logger.LogTrace("Request {Id} was cancelled", id);
                cts.Cancel();
            }
            else
            {
                _logger.LogDebug("Request {Id} was not found to cancel, stubbing it in.", id);
            }
        }

        public void StartRequest(object id)
        {
            _requests.TryAdd(GetId(id), new CancellationTokenSource());
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

            return id?.ToString();
        }

        Task IRequestRouter.RouteNotification(Notification notification, CancellationToken token)
        {
            return RouteNotification(GetDescriptor(notification), notification, token);
        }

        Task<ErrorResponse> IRequestRouter.RouteRequest(Request request, CancellationToken token)
        {
            return RouteRequest(GetDescriptor(request), request, token);
        }

        public abstract TDescriptor GetDescriptor(Notification notification);
        public abstract TDescriptor GetDescriptor(Request request);

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

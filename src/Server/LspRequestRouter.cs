using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.JsonRpc.Server.Messages;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Server.Abstractions;
using OmniSharp.Extensions.LanguageServer.Server.Messages;
using ISerializer = OmniSharp.Extensions.LanguageServer.Protocol.Serialization.ISerializer;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    internal class LspRequestRouter : ILspRequestRouter
    {
        private readonly IHandlerCollection _collection;
        private readonly IEnumerable<IHandlerMatcher> _handlerMatchers;
        private readonly ISerializer _serializer;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ConcurrentDictionary<string, CancellationTokenSource> _requests = new ConcurrentDictionary<string, CancellationTokenSource>();
        private readonly ILogger<LspRequestRouter> _logger;

        public LspRequestRouter(
            IHandlerCollection collection,
            ILoggerFactory loggerFactory,
            IEnumerable<IHandlerMatcher> handlerMatchers,
            ISerializer serializer,
            IServiceScopeFactory serviceScopeFactory)
        {
            _collection = collection;
            _handlerMatchers = handlerMatchers;
            _serializer = serializer;
            _serviceScopeFactory = serviceScopeFactory;
            _logger = loggerFactory.CreateLogger<LspRequestRouter>();
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

        private ILspHandlerDescriptor FindDescriptor(IMethodWithParams instance)
        {
            return FindDescriptor(instance.Method, instance.Params);
        }

        private ILspHandlerDescriptor FindDescriptor(string method, JToken @params)
        {
            _logger.LogDebug("Finding descriptor for {Method}", method);
            var descriptor = _collection.FirstOrDefault(x => x.Method == method);
            if (descriptor is null)
            {
                _logger.LogDebug("Unable to find {Method}, methods found include {Methods}", method, string.Join(", ", _collection.Select(x => x.Method + ":" + x.HandlerType.FullName)));
                return null;
            }

            if (@params == null || descriptor.Params == null) return descriptor;

            var paramsValue = @params.ToObject(descriptor.Params, _serializer.JsonSerializer);

            var lspHandlerDescriptors = _collection.Where(handler => handler.Method == method).ToList();

            return _handlerMatchers.SelectMany(strat => strat.FindHandler(paramsValue, lspHandlerDescriptors)).FirstOrDefault() ?? descriptor;
        }

        public async Task RouteNotification(IHandlerDescriptor descriptor, Notification notification)
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
                            await MediatRHandlers.HandleNotification(mediator, descriptor, EmptyRequest.Instance, CancellationToken.None);
                        }
                        else
                        {
                            _logger.LogDebug("Converting params for Notification {Method} to {Type}", notification.Method, descriptor.Params.FullName);
                            var @params = notification.Params.ToObject(descriptor.Params, _serializer.JsonSerializer);

                            await MediatRHandlers.HandleNotification(mediator, descriptor, @params ?? EmptyRequest.Instance, CancellationToken.None);
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogCritical(Events.UnhandledRequest, e, "Failed to handle request {Method}", notification.Method);
                    }
                }
            }
        }

        public async Task<ErrorResponse> RouteRequest(IHandlerDescriptor descriptor, Request request)
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
                    var cts = new CancellationTokenSource();
                    _requests.TryAdd(id, cts);

                    // TODO: Try / catch for Internal Error
                    try
                    {
                        if (descriptor is null)
                        {
                            _logger.LogDebug("descriptor not found for Request ({Id}) {Method}", request.Id, request.Method);
                            return new MethodNotFound(request.Id, request.Method);
                        }

                        object @params;
                        try
                        {
                            _logger.LogDebug("Converting params for Request ({Id}) {Method} to {Type}", request.Id, request.Method, descriptor.Params.FullName);
                            @params = request.Params?.ToObject(descriptor.Params, _serializer.JsonSerializer);
                        }
                        catch (Exception cannotDeserializeRequestParams)
                        {
                            _logger.LogError(new EventId(-32602), cannotDeserializeRequestParams, "Failed to deserialise request parameters.");
                            return new InvalidParams(request.Id);
                        }

                        var result = MediatRHandlers.HandleRequest(mediator, descriptor, @params ?? EmptyRequest.Instance , cts.Token);
                        await result;

                        _logger.LogDebug("Result was {Type}", result.GetType().FullName);

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
                            _logger.LogDebug("Response value was {Type}", responseValue?.GetType().FullName);
                        }

                        return new JsonRpc.Client.Response(request.Id, responseValue);
                    }
                    catch (TaskCanceledException e)
                    {
                        _logger.LogDebug("Request {Id} was cancelled", id);
                        return new RequestCancelled();
                    }
                    catch (Exception e)
                    {
                        _logger.LogCritical(Events.UnhandledRequest, e, "Failed to handle notification {Method}", request.Method);
                        return new InternalError(id);
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
                cts.Cancel();
            }
            else
            {
                _logger.LogDebug("Request {Id} was not found to cancel", id);
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
            return RouteNotification(FindDescriptor(notification), notification);
        }

        Task<ErrorResponse> IRequestRouter.RouteRequest(Request request)
        {
            return RouteRequest(FindDescriptor(request), request);
        }
    }
}

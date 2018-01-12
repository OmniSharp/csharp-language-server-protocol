using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.JsonRpc.Server.Messages;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Server.Abstractions;
using OmniSharp.Extensions.LanguageServer.Server.Messages;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    internal class LspRequestRouter : IRequestRouter
    {
        private readonly IHandlerCollection _collection;
        private readonly IEnumerable<IHandlerMatcher> _routeMatchers;
        private readonly ISerializer _serializer;
        private readonly ConcurrentDictionary<string, CancellationTokenSource> _requests = new ConcurrentDictionary<string, CancellationTokenSource>();
        private readonly ILogger<LspRequestRouter> _logger;

        public LspRequestRouter(IHandlerCollection collection,
            ILoggerFactory loggerFactory,
            IHandlerMatcherCollection routeMatchers,
            ISerializer serializer)
        {
            _collection = collection;
            _routeMatchers = routeMatchers;
            _serializer = serializer;
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
                _logger.LogDebug("Unable to find {Method}, methods found include {Methods}", method, string.Join(", ", _collection.Select(x => x.Method + ":" + x.Handler.GetType().FullName)));
                return null;
            }

            if (@params == null || descriptor.Params == null) return descriptor;

            var paramsValue = @params.ToObject(descriptor.Params, _serializer.JsonSerializer);

            var lspHandlerDescriptors = _collection.Where(handler => handler.Method == method).ToList();

            return _routeMatchers.SelectMany(strat => strat.FindHandler(paramsValue, lspHandlerDescriptors)).FirstOrDefault() ?? descriptor;
        }

        public async Task RouteNotification(IHandlerDescriptor handler, Notification notification)
        {
            try
            {
                Task result;
                if (handler.Params is null)
                {
                    result = ReflectionRequestHandlers.HandleNotification(handler);
                }
                else
                {
                    var @params = notification.Params.ToObject(handler.Params, _serializer.JsonSerializer);
                    result = ReflectionRequestHandlers.HandleNotification(handler, @params);
                }

                await result;
            }
            catch (Exception e)
            {
                _logger.LogCritical(Events.UnhandledRequest, e, "Failed to handle request {Method}", notification.Method);
            }
        }

        public async Task<ErrorResponse> RouteRequest(IHandlerDescriptor descriptor, Request request)
        {
            var id = GetId(request.Id);
            var cts = new CancellationTokenSource();
            _requests.TryAdd(id, cts);

            // TODO: Try / catch for Internal Error
            try
            {
                if (descriptor is null)
                {
                    return new MethodNotFound(request.Id, request.Method);
                }

                object @params;
                try
                {
                    @params = request.Params?.ToObject(descriptor.Params, _serializer.JsonSerializer);
                }
                catch
                {
                    return new InvalidParams(request.Id);
                }

                var result = ReflectionRequestHandlers.HandleRequest(descriptor, @params, cts.Token).ConfigureAwait(false);
                await result;

                object responseValue = null;
                if (result.GetType().GetTypeInfo().IsGenericType)
                {
                    var property = typeof(Task<>)
                        .MakeGenericType(result.GetType().GetTypeInfo().GetGenericArguments()[0]).GetTypeInfo()
                        .GetProperty(nameof(Task<object>.Result), BindingFlags.Public | BindingFlags.Instance);

                    responseValue = property.GetValue(result);
                }

                return new JsonRpc.Client.Response(request.Id, responseValue);
            }
            catch (TaskCanceledException)
            {
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

        public void CancelRequest(object id)
        {
            if (_requests.TryGetValue(GetId(id), out var cts))
            {
                cts.Cancel();
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

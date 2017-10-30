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
using OmniSharp.Extensions.LanguageServer.Abstractions;
using OmniSharp.Extensions.LanguageServer.Messages;
using OmniSharp.Extensions.LanguageServer.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;

namespace OmniSharp.Extensions.LanguageServer
{
    class LspRequestRouter : IRequestRouter
    {
        private readonly IHandlerCollection _collection;
        private readonly ConcurrentDictionary<string, CancellationTokenSource> _requests = new ConcurrentDictionary<string, CancellationTokenSource>();
        private readonly ILogger<LspRequestRouter> _logger;

        public LspRequestRouter(IHandlerCollection collection, ILoggerFactory loggerFactory)
        {
            _collection = collection;
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

            if (@params != null && descriptor.Params != null)
            {
                var paramsValue = @params.ToObject(descriptor.Params);
                if (paramsValue is ITextDocumentIdentifierParams textDocumentIdentifierParams)
                {
                    var attributes = GetTextDocumentAttributes(textDocumentIdentifierParams.TextDocument.Uri);

                    _logger.LogTrace("Found attributes {Count}, {Attributes}", attributes.Count, attributes.Select(x => $"{x.LanguageId}:{x.Scheme}:{x.Uri}"));

                    return GetHandler(method, attributes);
                }
                else if (paramsValue is DidOpenTextDocumentParams openTextDocumentParams)
                {
                    var attributes = new TextDocumentAttributes(openTextDocumentParams.TextDocument.Uri, openTextDocumentParams.TextDocument.LanguageId);

                    _logger.LogTrace("Created attribute {Attribute}", $"{attributes.LanguageId}:{attributes.Scheme}:{attributes.Uri}");

                    return GetHandler(method, attributes);
                }
                else if (paramsValue is DidChangeTextDocumentParams didChangeDocumentParams)
                {
                    // TODO: Do something with document version here?
                    var attributes = GetTextDocumentAttributes(didChangeDocumentParams.TextDocument.Uri);

                    _logger.LogTrace("Found attributes {Count}, {Attributes}", attributes.Count, attributes.Select(x => $"{x.LanguageId}:{x.Scheme}:{x.Uri}"));

                    return GetHandler(method, attributes);
                }
            }

            // TODO: How to split these
            // Do they fork and join?
            return descriptor;
        }

        private List<TextDocumentAttributes> GetTextDocumentAttributes(Uri uri)
        {
            var textDocumentSyncHandlers = _collection
                .Select(x => x.Handler is ITextDocumentSyncHandler r ? r : null)
                .Where(x => x != null)
                .Distinct();
            return textDocumentSyncHandlers
                .Select(x => x.GetTextDocumentAttributes(uri))
                .Where(x => x != null)
                .Distinct()
                .ToList();
        }

        private ILspHandlerDescriptor GetHandler(string method, IEnumerable<TextDocumentAttributes> attributes)
        {
            return attributes
                .Select(x => GetHandler(method, x))
                .FirstOrDefault(x => x != null);
        }

        private ILspHandlerDescriptor GetHandler(string method, TextDocumentAttributes attributes)
        {
            _logger.LogTrace("Looking for handler for method {Method}", method);
            foreach (var handler in _collection.Where(x => x.Method == method))
            {
                _logger.LogTrace("Checking handler {Method}:{Handler}", method, handler.Handler.GetType().FullName);
                var registrationOptions = handler.Registration.RegisterOptions as TextDocumentRegistrationOptions;

                _logger.LogTrace("Registration options {OptionsName}", registrationOptions.GetType().FullName);
                _logger.LogTrace("Document Selector {DocumentSelector}", registrationOptions.DocumentSelector.ToString());
                if (registrationOptions.DocumentSelector == null || registrationOptions.DocumentSelector.IsMatch(attributes))
                {
                    _logger.LogTrace("Handler Selected: {Handler} via {DocumentSelector} (targeting {HandlerInterface})", handler.Handler.GetType().FullName, registrationOptions.DocumentSelector.ToString(), handler.HandlerType.GetType().FullName);
                    return handler;
                }
            }
            return null;
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
                    var @params = notification.Params.ToObject(handler.Params);
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

                Task result;
                if (descriptor.Params is null)
                {
                    result = ReflectionRequestHandlers.HandleRequest(descriptor, cts.Token);
                }
                else
                {
                    object @params;
                    try
                    {
                        @params = request.Params.ToObject(descriptor.Params);
                    }
                    catch
                    {
                        return new InvalidParams(request.Id);
                    }

                    result = ReflectionRequestHandlers.HandleRequest(descriptor, @params, cts.Token);
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

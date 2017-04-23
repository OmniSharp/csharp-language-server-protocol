using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using JsonRpc;
using JsonRpc.Server;
using JsonRpc.Server.Messages;
using Lsp.Handlers;
using Lsp.Messages;
using Lsp.Protocol;
using Newtonsoft.Json.Linq;

namespace Lsp
{
    class LspRequestRouter : IRequestRouter
    {
        private readonly IHandlerCollection _collection;
        private readonly ConcurrentDictionary<string, CancellationTokenSource> _requests = new ConcurrentDictionary<string, CancellationTokenSource>();

        public LspRequestRouter(IHandlerCollection collection)
        {
            _collection = collection;
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

        public async void RouteNotification(Notification notification)
        {
            var handler = _collection.Get(notification.Method);

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

        public async Task<ErrorResponse> RouteRequest(Request request)
        {
            var id = GetId(request.Id);
            var cts = new CancellationTokenSource();
            _requests.TryAdd(id, cts);

            // TODO: Try / catch for Internal Error
            try
            {
                var handler = _collection.Get(request.Method);

                var method = _collection.Get(request.Method);
                if (method is null)
                {
                    return new MethodNotFound(request.Id);
                }

                Task result;
                if (method.Params is null)
                {
                    result = ReflectionRequestHandlers.HandleRequest(handler, cts.Token);
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

                    result = ReflectionRequestHandlers.HandleRequest(handler, @params, cts.Token);
                }

                await result.ConfigureAwait(false);

                object responseValue = null;
                if (result.GetType().GetTypeInfo().IsGenericType)
                {
                    var property = typeof(Task<>)
                        .MakeGenericType(result.GetType().GetTypeInfo().GetGenericArguments()[0])
                        .GetProperty(nameof(Task<object>.Result), BindingFlags.Public | BindingFlags.Instance);

                    responseValue = property.GetValue(result);
                }

                return new JsonRpc.Client.Response(request.Id, responseValue);
            }
            catch (TaskCanceledException)
            {
                return new RequestCancelled();
            }
            finally
            {
                _requests.TryRemove(id, out var _);
            }
        }

        public IDisposable Add(IJsonRpcHandler handler)
        {
            return _collection.Add(handler);
        }

        public void Remove(IJsonRpcHandler handler)
        {
            _collection.Remove(handler);
        }

        public void CancelRequest(object id)
        {
            if (_requests.TryGetValue(GetId(id), out var cts))
            {
                cts.Cancel();
            }
        }
    }

    public static class LspHelper
    {
        private static readonly ConcurrentDictionary<Type, string> MethodNames = new ConcurrentDictionary<Type, string>();

        public static string GetMethodName<T>()
            where T : IJsonRpcHandler
        {
            return GetMethodName(typeof(T));
        }

        public static string GetMethodName(Type type)
        {
            if (MethodNames.TryGetValue(type, out var method)) return method;

            // Custom method
            var attribute = type.GetTypeInfo().GetCustomAttribute<MethodAttribute>();
            if (attribute is null)
            {
                attribute = type.GetTypeInfo()
                    .ImplementedInterfaces
                    .Select(t => t.GetTypeInfo().GetCustomAttribute<MethodAttribute>())
                    .FirstOrDefault(x => x != null);
            }

            // TODO: Log unknown method name
            if (attribute is null)
            {

            }

            MethodNames.TryAdd(type, attribute.Method);
            return attribute.Method;
        }
    }

    class RequestProcessIdentifier : IRequestProcessIdentifier
    {
        public RequestProcessType Identify(Renor renor)
        {
            // TODO: Update to it infer based on incoming messages
            return RequestProcessType.Serial;
        }
    }

    class LspReciever : Reciever
    {
        private bool _initialized;

        public override (IEnumerable<Renor> results, bool hasResponse) GetRequests(JToken container)
        {
            if (_initialized) return base.GetRequests(container);

            var newResults = new List<Renor>();

            // Based on https://github.com/Microsoft/language-server-protocol/blob/master/protocol.md#initialize-request
            var (results, hasResponse) = base.GetRequests(container);
            foreach (var item in results)
            {
                if (item.IsRequest && item.Request.Method == LspHelper.GetMethodName<IInitializeHandler>())
                {
                    newResults.Add(item);
                }
                else if (item.IsRequest)
                {
                    newResults.Add(new ServerNotInitialized());
                }
                else if (item.IsResponse)
                {
                    newResults.Add(item);
                }
            }

            return (newResults, hasResponse);
        }

        public void Initialized()
        {
            _initialized = true;
        }
    }

    public class LanguageServer
    {
        private readonly Connection _connection;
        private readonly LspRequestRouter _requestRouter;

        public LanguageServer(TextReader input, TextWriter output)
        {
            var collection = new HandlerCollection();
            var outputHandler = new OutputHandler(output);
            _requestRouter = new LspRequestRouter(collection);
            _connection = new Connection(
                input,
                outputHandler,
                new Reciever(),
                new RequestProcessIdentifier(),
                new LspRequestRouter(collection),
                new ResponseRouter(outputHandler));

            AddHandler(new CancelRequestHandler(_requestRouter));
        }

        internal LanguageServer(
            TextReader input,
            IOutputHandler output,
            IReciever reciever,
            IRequestProcessIdentifier requestProcessIdentifier,
            IRequestRouter requestRouter,
            IResponseRouter responseRouter
            )
        {
            var collection = new HandlerCollection();

            _connection = new Connection(input, output, reciever, requestProcessIdentifier, requestRouter, responseRouter);
            _requestRouter = new LspRequestRouter(collection);

            AddHandler(new CancelRequestHandler(_requestRouter));
        }

        public IDisposable AddHandler(IJsonRpcHandler handler)
        {
            return _requestRouter.Add(handler);
        }

        public void RemoveHandler(IJsonRpcHandler handler)
        {
            _requestRouter.Remove(handler);
        }

        public async Task Initialize()
        {
            _connection.Open();


        }
    }
}
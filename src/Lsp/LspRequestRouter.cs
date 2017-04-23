using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using JsonRpc;
using JsonRpc.Server;
using JsonRpc.Server.Messages;
using Lsp.Messages;

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
}
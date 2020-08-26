using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace OmniSharp.Extensions.JsonRpc
{
    public class JsonRpcHandlerOptions
    {
        public RequestProcessType RequestProcessType { get; set; }
    }

    public interface IJsonRpcHandlerRegistry
    {
    }

    public interface IJsonRpcHandlerRegistry<out T> : IJsonRpcHandlerRegistry where T : IJsonRpcHandlerRegistry<T>
    {
        T AddHandler(string method, IJsonRpcHandler handler, JsonRpcHandlerOptions? options = null);
        T AddHandler(string method, JsonRpcHandlerFactory handlerFunc, JsonRpcHandlerOptions? options = null);
        T AddHandlers(params IJsonRpcHandler[] handlers);
        T AddHandler(JsonRpcHandlerFactory handlerFunc, JsonRpcHandlerOptions? options = null);
        T AddHandler(IJsonRpcHandler handler, JsonRpcHandlerOptions? options = null);
        T AddHandler<TTHandler>(JsonRpcHandlerOptions? options = null) where TTHandler : IJsonRpcHandler;
        T AddHandler<TTHandler>(string method, JsonRpcHandlerOptions? options = null) where TTHandler : IJsonRpcHandler;
        T AddHandler(Type type, JsonRpcHandlerOptions? options = null);
        T AddHandler(string method, Type type, JsonRpcHandlerOptions? options = null);
        T AddHandlerLink(string fromMethod, string toMethod);

        T OnJsonRequest(string method, Func<JToken, Task<JToken>> handler, JsonRpcHandlerOptions? options = null);
        T OnJsonRequest(string method, Func<JToken, CancellationToken, Task<JToken>> handler, JsonRpcHandlerOptions? options = null);
        T OnRequest<TParams, TResponse>(string method, Func<TParams, Task<TResponse>> handler, JsonRpcHandlerOptions? options = null);
        T OnRequest<TResponse>(string method, Func<Task<TResponse>> handler, JsonRpcHandlerOptions? options = null);
        T OnRequest<TParams, TResponse>(string method, Func<TParams, CancellationToken, Task<TResponse>> handler, JsonRpcHandlerOptions? options = null);
        T OnRequest<TResponse>(string method, Func<CancellationToken, Task<TResponse>> handler, JsonRpcHandlerOptions? options = null);
        T OnRequest<TParams>(string method, Func<TParams, Task> handler, JsonRpcHandlerOptions? options = null);
        T OnRequest<TParams>(string method, Func<TParams, CancellationToken, Task> handler, JsonRpcHandlerOptions? options = null);
        T OnRequest<TParams>(string method, Func<CancellationToken, Task> handler, JsonRpcHandlerOptions? options = null);
        T OnJsonNotification(string method, Action<JToken> handler, JsonRpcHandlerOptions? options = null);
        T OnJsonNotification(string method, Action<JToken, CancellationToken> handler, JsonRpcHandlerOptions? options = null);
        T OnJsonNotification(string method, Func<JToken, Task> handler, JsonRpcHandlerOptions? options = null);
        T OnJsonNotification(string method, Func<JToken, CancellationToken, Task> handler, JsonRpcHandlerOptions? options = null);
        T OnNotification<TParams>(string method, Action<TParams, CancellationToken> handler, JsonRpcHandlerOptions? options = null);
        T OnNotification<TParams>(string method, Action<TParams> handler, JsonRpcHandlerOptions? options = null);
        T OnNotification<TParams>(string method, Func<TParams, CancellationToken, Task> handler, JsonRpcHandlerOptions? options = null);
        T OnNotification<TParams>(string method, Func<TParams, Task> handler, JsonRpcHandlerOptions? options = null);
        T OnNotification(string method, Action handler, JsonRpcHandlerOptions? options = null);
        T OnNotification(string method, Func<CancellationToken, Task> handler, JsonRpcHandlerOptions? options = null);
        T OnNotification(string method, Func<Task> handler, JsonRpcHandlerOptions? options = null);
    }
}

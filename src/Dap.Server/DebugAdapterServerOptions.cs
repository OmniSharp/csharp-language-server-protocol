using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Server;
using OmniSharp.Extensions.DebugAdapter.Shared;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Server
{
    public class DebugAdapterServerOptions : DebugAdapterRpcOptionsBase<DebugAdapterServerOptions>, IDebugAdapterServerRegistry
    {
        public DebugAdapterServerOptions()
        {
            WithAssemblies(typeof(DebugAdapterServerOptions).Assembly, typeof(DebugAdapterRequestRouter).Assembly);
        }
        public Capabilities Capabilities { get; set; } = new Capabilities();

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.AddHandler(string method, IJsonRpcHandler handler, JsonRpcHandlerOptions? options) =>
            AddHandler(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.
            AddHandler(string method, JsonRpcHandlerFactory handlerFunc, JsonRpcHandlerOptions? options) => AddHandler(method, handlerFunc, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.AddHandlers(params IJsonRpcHandler[] handlers) => AddHandlers(handlers);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.AddHandler(JsonRpcHandlerFactory handlerFunc, JsonRpcHandlerOptions? options) =>
            AddHandler(handlerFunc, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.AddHandler(IJsonRpcHandler handler, JsonRpcHandlerOptions? options) =>
            AddHandler(handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.AddHandler<TTHandler>(JsonRpcHandlerOptions? options) => AddHandler<TTHandler>(options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.AddHandler<TTHandler>(string method, JsonRpcHandlerOptions? options) =>
            AddHandler<TTHandler>(method, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.AddHandler(Type type, JsonRpcHandlerOptions? options) => AddHandler(type, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.AddHandler(string method, Type type, JsonRpcHandlerOptions? options) =>
            AddHandler(method, type, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.AddHandlerLink(string fromMethod, string toMethod) =>
            AddHandlerLink(fromMethod, toMethod);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnJsonRequest(
            string method, Func<JToken, Task<JToken>> handler, JsonRpcHandlerOptions? options
        ) => OnJsonRequest(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnJsonRequest(
            string method, Func<JToken, CancellationToken, Task<JToken>> handler, JsonRpcHandlerOptions? options
        ) => OnJsonRequest(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnRequest<TParams, TResponse>(
            string method, Func<TParams, Task<TResponse>> handler, JsonRpcHandlerOptions? options
        ) => OnRequest(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnRequest<TParams, TResponse>(
            string method, Func<TParams, CancellationToken, Task<TResponse>> handler, JsonRpcHandlerOptions? options
        ) => OnRequest(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnRequest<TResponse>(
            string method, Func<Task<TResponse>> handler, JsonRpcHandlerOptions? options
        ) => OnRequest(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnRequest<TResponse>(
            string method, Func<CancellationToken, Task<TResponse>> handler, JsonRpcHandlerOptions? options
        ) => OnRequest(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnRequest<TParams>(
            string method, Func<TParams, Task> handler, JsonRpcHandlerOptions? options
        ) => OnRequest(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnRequest<TParams>(
            string method, Func<TParams, CancellationToken, Task> handler, JsonRpcHandlerOptions? options
        ) => OnRequest(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnRequest<TParams>(
            string method, Func<CancellationToken, Task> handler, JsonRpcHandlerOptions? options
        ) => OnRequest<TParams>(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnNotification<TParams>(
            string method, Action<TParams, CancellationToken> handler, JsonRpcHandlerOptions? options
        ) => OnNotification(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnJsonNotification(string method, Action<JToken> handler, JsonRpcHandlerOptions? options) =>
            OnJsonNotification(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnJsonNotification(
            string method, Func<JToken, CancellationToken, Task> handler,
            JsonRpcHandlerOptions? options
        ) => OnJsonNotification(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnJsonNotification(
            string method, Func<JToken, Task> handler,
            JsonRpcHandlerOptions? options
        ) => OnJsonNotification(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnJsonNotification(
            string method, Action<JToken, CancellationToken> handler,
            JsonRpcHandlerOptions? options
        ) => OnJsonNotification(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnNotification<TParams>(
            string method, Action<TParams> handler,
            JsonRpcHandlerOptions? options
        ) => OnNotification(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnNotification<TParams>(
            string method, Func<TParams, CancellationToken, Task> handler,
            JsonRpcHandlerOptions? options
        ) => OnNotification(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnNotification<TParams>(
            string method, Func<TParams, Task> handler,
            JsonRpcHandlerOptions? options
        ) => OnNotification(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnNotification(string method, Action handler, JsonRpcHandlerOptions? options) =>
            OnNotification(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnNotification(
            string method, Func<CancellationToken, Task> handler,
            JsonRpcHandlerOptions? options
        ) => OnNotification(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnNotification(string method, Func<Task> handler, JsonRpcHandlerOptions? options) =>
            OnNotification(method, handler, options);
    }
}

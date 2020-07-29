using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Client
{
    public class DebugAdapterClientOptions : DebugAdapterRpcOptionsBase<DebugAdapterClientOptions>, IDebugAdapterClientRegistry, IInitializeRequestArguments
    {
        internal readonly List<OnClientStartedDelegate> StartedDelegates = new List<OnClientStartedDelegate>();
        public ISerializer Serializer { get; set; } = new DapSerializer();
        public override IRequestProcessIdentifier RequestProcessIdentifier { get; set; } = new ParallelRequestProcessIdentifier();
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public string AdapterId { get; set; }
        public string Locale { get; set; }
        public bool? LinesStartAt1 { get; set; }
        public bool? ColumnsStartAt1 { get; set; }
        public string PathFormat { get; set; }
        public bool? SupportsVariableType { get; set; }
        public bool? SupportsVariablePaging { get; set; }
        public bool? SupportsRunInTerminalRequest { get; set; }
        public bool? SupportsMemoryReferences { get; set; }
        public bool? SupportsProgressReporting { get; set; }
        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.AddHandler(string method, IJsonRpcHandler handler, JsonRpcHandlerOptions options) => this.AddHandler(method, handler, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.AddHandler(string method, Func<IServiceProvider, IJsonRpcHandler> handlerFunc, JsonRpcHandlerOptions options) => this.AddHandler(method, handlerFunc, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.AddHandlers(params IJsonRpcHandler[] handlers) => this.AddHandlers(handlers);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.AddHandler<THandler>(Func<IServiceProvider, THandler> handlerFunc, JsonRpcHandlerOptions options) => this.AddHandler(handlerFunc, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.AddHandler<THandler>(THandler handler, JsonRpcHandlerOptions options) => this.AddHandler(handler, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.AddHandler<TTHandler>(JsonRpcHandlerOptions options) => this.AddHandler<TTHandler>(options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.AddHandler<TTHandler>(string method, JsonRpcHandlerOptions options) => this.AddHandler<TTHandler>(method, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.AddHandler(Type type, JsonRpcHandlerOptions options) => this.AddHandler(type, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.AddHandler(string method, Type type, JsonRpcHandlerOptions options) => this.AddHandler(method, type, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.OnJsonRequest(string method, Func<JToken, Task<JToken>> handler, JsonRpcHandlerOptions options) => OnJsonRequest(method, handler, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.OnJsonRequest(string method, Func<JToken, CancellationToken, Task<JToken>> handler, JsonRpcHandlerOptions options) => OnJsonRequest(method, handler, options);
        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.OnRequest<TParams, TResponse>(string method, Func<TParams, Task<TResponse>> handler, JsonRpcHandlerOptions options) => OnRequest(method, handler, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.OnRequest<TParams, TResponse>(string method, Func<TParams, CancellationToken, Task<TResponse>> handler, JsonRpcHandlerOptions options) => OnRequest(method, handler, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.OnRequest<TResponse>(string method, Func<Task<TResponse>> handler, JsonRpcHandlerOptions options) => OnRequest(method, handler, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.OnRequest<TResponse>(string method, Func<CancellationToken, Task<TResponse>> handler, JsonRpcHandlerOptions options) => OnRequest(method, handler, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.OnRequest<TParams>(string method, Func<TParams, Task> handler, JsonRpcHandlerOptions options) => OnRequest(method, handler, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.OnRequest<TParams>(string method, Func<TParams, CancellationToken, Task> handler, JsonRpcHandlerOptions options) => OnRequest(method, handler, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.OnRequest<TParams>(string method, Func<CancellationToken, Task> handler, JsonRpcHandlerOptions options) => OnRequest<TParams>(method, handler, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.OnNotification<TParams>(string method, Action<TParams, CancellationToken> handler, JsonRpcHandlerOptions options) => OnNotification(method, handler, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.OnJsonNotification(string method, Action<JToken> handler, JsonRpcHandlerOptions options) => OnJsonNotification(method, handler, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.OnJsonNotification(string method, Func<JToken, CancellationToken, Task> handler, JsonRpcHandlerOptions options) => OnJsonNotification(method, handler, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.OnJsonNotification(string method, Func<JToken, Task> handler, JsonRpcHandlerOptions options) => OnJsonNotification(method, handler, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.OnJsonNotification(string method, Action<JToken, CancellationToken> handler, JsonRpcHandlerOptions options) => OnJsonNotification(method, handler, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.OnNotification<TParams>(string method, Action<TParams> handler, JsonRpcHandlerOptions options) => OnNotification(method, handler, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.OnNotification<TParams>(string method, Func<TParams, CancellationToken, Task> handler, JsonRpcHandlerOptions options) => OnNotification(method, handler, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.OnNotification<TParams>(string method, Func<TParams, Task> handler, JsonRpcHandlerOptions options) => OnNotification(method, handler, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.OnNotification(string method, Action handler, JsonRpcHandlerOptions options) => OnNotification(method, handler, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.OnNotification(string method, Func<CancellationToken, Task> handler, JsonRpcHandlerOptions options) => OnNotification(method, handler, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.OnNotification(string method, Func<Task> handler, JsonRpcHandlerOptions options) => OnNotification(method, handler, options);
    }
}
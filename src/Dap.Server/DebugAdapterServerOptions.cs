﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Server
{
    public class DebugAdapterServerOptions : DebugAdapterRpcOptionsBase<DebugAdapterServerOptions>, IDebugAdapterServerRegistry
    {
        public Capabilities Capabilities { get; set; } = new Capabilities();
        internal readonly List<OnServerStartedDelegate> StartedDelegates = new List<OnServerStartedDelegate>();
        internal readonly List<InitializedDelegate> InitializedDelegates = new List<InitializedDelegate>();
        internal readonly List<InitializeDelegate> InitializeDelegates = new List<InitializeDelegate>();
        public ISerializer Serializer { get; set; } = new DapSerializer();
        public override IRequestProcessIdentifier RequestProcessIdentifier { get; set; } = new ParallelRequestProcessIdentifier();

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.AddHandler(string method, IJsonRpcHandler handler, JsonRpcHandlerOptions options) =>
            this.AddHandler(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.AddHandler(string method, Func<IServiceProvider, IJsonRpcHandler> handlerFunc,
            JsonRpcHandlerOptions options) => this.AddHandler(method, handlerFunc, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.AddHandlers(params IJsonRpcHandler[] handlers) => this.AddHandlers(handlers);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.AddHandler<THandler>(Func<IServiceProvider, THandler> handlerFunc,
            JsonRpcHandlerOptions options) => this.AddHandler(handlerFunc, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.AddHandler<THandler>(THandler handler, JsonRpcHandlerOptions options) =>
            this.AddHandler(handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.AddHandler<TTHandler>(JsonRpcHandlerOptions options) =>
            this.AddHandler<TTHandler>(options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.AddHandler<TTHandler>(string method, JsonRpcHandlerOptions options) =>
            this.AddHandler<TTHandler>(method, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.AddHandler(Type type, JsonRpcHandlerOptions options) => this.AddHandler(type, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.AddHandler(string method, Type type, JsonRpcHandlerOptions options) =>
            this.AddHandler(method, type, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnJsonRequest(string method, Func<JToken, Task<JToken>> handler,
            JsonRpcHandlerOptions options) => OnJsonRequest(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnJsonRequest(string method, Func<JToken, CancellationToken, Task<JToken>> handler,
            JsonRpcHandlerOptions options) => OnJsonRequest(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnRequest<TParams, TResponse>(string method, Func<TParams, Task<TResponse>> handler,
            JsonRpcHandlerOptions options) => OnRequest(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnRequest<TParams, TResponse>(string method,
            Func<TParams, CancellationToken, Task<TResponse>> handler, JsonRpcHandlerOptions options) => OnRequest(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnRequest<TResponse>(string method, Func<Task<TResponse>> handler,
            JsonRpcHandlerOptions options) => OnRequest(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnRequest<TResponse>(string method, Func<CancellationToken, Task<TResponse>> handler,
            JsonRpcHandlerOptions options) => OnRequest(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnRequest<TParams>(string method, Func<TParams, Task> handler,
            JsonRpcHandlerOptions options) => OnRequest(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnRequest<TParams>(string method, Func<TParams, CancellationToken, Task> handler,
            JsonRpcHandlerOptions options) => OnRequest(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnRequest<TParams>(string method, Func<CancellationToken, Task> handler,
            JsonRpcHandlerOptions options) => OnRequest<TParams>(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnNotification<TParams>(string method, Action<TParams, CancellationToken> handler,
            JsonRpcHandlerOptions options) => OnNotification(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnJsonNotification(string method, Action<JToken> handler, JsonRpcHandlerOptions options) =>
            OnJsonNotification(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnJsonNotification(string method, Func<JToken, CancellationToken, Task> handler,
            JsonRpcHandlerOptions options) => OnJsonNotification(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnJsonNotification(string method, Func<JToken, Task> handler,
            JsonRpcHandlerOptions options) => OnJsonNotification(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnJsonNotification(string method, Action<JToken, CancellationToken> handler,
            JsonRpcHandlerOptions options) => OnJsonNotification(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnNotification<TParams>(string method, Action<TParams> handler,
            JsonRpcHandlerOptions options) => OnNotification(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnNotification<TParams>(string method, Func<TParams, CancellationToken, Task> handler,
            JsonRpcHandlerOptions options) => OnNotification(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnNotification<TParams>(string method, Func<TParams, Task> handler,
            JsonRpcHandlerOptions options) => OnNotification(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnNotification(string method, Action handler, JsonRpcHandlerOptions options) =>
            OnNotification(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnNotification(string method, Func<CancellationToken, Task> handler,
            JsonRpcHandlerOptions options) => OnNotification(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnNotification(string method, Func<Task> handler, JsonRpcHandlerOptions options) =>
            OnNotification(method, handler, options);
    }
}
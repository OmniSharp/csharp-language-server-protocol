using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Reactive.Disposables;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;
using OmniSharp.Extensions.LanguageServer.Shared;
using ISerializer = OmniSharp.Extensions.LanguageServer.Protocol.Serialization.ISerializer;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public class LanguageServerOptions : LanguageProtocolRpcOptionsBase<LanguageServerOptions>, ILanguageServerRegistry
    {
        public ServerInfo ServerInfo { get; set; }
        public ISerializer Serializer { get; set; } = new Protocol.Serialization.Serializer(ClientVersion.Lsp3);
        public ILspServerReceiver Receiver { get; set; } = new LspServerReceiver();

        internal readonly List<InitializeDelegate> InitializeDelegates = new List<InitializeDelegate>();
        internal readonly List<InitializedDelegate> InitializedDelegates = new List<InitializedDelegate>();
        internal readonly List<OnServerStartedDelegate> StartedDelegates = new List<OnServerStartedDelegate>();

        public LanguageServerOptions WithReceiver(ILspServerReceiver receiver)
        {
            Receiver = receiver;
            return this;
        }

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.AddHandler(string method, IJsonRpcHandler handler) => this.AddHandler(method, handler);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.AddHandler(string method, Func<IServiceProvider, IJsonRpcHandler> handlerFunc) => this.AddHandler(method, handlerFunc);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.AddHandlers(params IJsonRpcHandler[] handlers) => this.AddHandlers(handlers);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.AddHandler<THandler>(Func<IServiceProvider, THandler> handlerFunc) => this.AddHandler(handlerFunc);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.AddHandler<THandler>(THandler handler) => this.AddHandler(handler);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.AddHandler<TTHandler>() => this.AddHandler<TTHandler>();

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.AddHandler<TTHandler>(string method) => this.AddHandler<TTHandler>(method);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.AddHandler(Type type) => this.AddHandler(type);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.AddHandler(string method, Type type) => this.AddHandler(method, type);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnRequest<TParams, TResponse>(string method, Func<TParams, Task<TResponse>> handler) => this.OnRequest(method, handler);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnRequest<TResponse>(string method, Func<Task<TResponse>> handler) => this.OnRequest(method, handler);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnRequest<TParams, TResponse>(string method, Func<TParams, CancellationToken, Task<TResponse>> handler) => this.OnRequest(method, handler);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnRequest<TResponse>(string method, Func<CancellationToken, Task<TResponse>> handler) => this.OnRequest(method, handler);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnRequest<TParams>(string method, Func<TParams, Task> handler) => this.OnRequest(method, handler);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnRequest<TParams>(string method, Func<TParams, CancellationToken, Task> handler) => this.OnRequest(method, handler);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnRequest<TParams>(string method, Func<CancellationToken, Task> handler) => this.OnRequest<TParams>(method, handler);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnNotification<TParams>(string method, Action<TParams, CancellationToken> handler) => this.OnNotification(method, handler);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnNotification(string method, Action handler) => this.OnNotification(method, handler);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnNotification<TParams>(string method, Action<TParams> handler) => this.OnNotification(method, handler);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnNotification<TParams>(string method, Func<TParams, CancellationToken, Task> handler) => this.OnNotification(method, handler);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnNotification<TParams>(string method, Func<TParams, Task> handler) => this.OnNotification(method, handler);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnNotification(string method, Func<CancellationToken, Task> handler) => this.OnNotification(method, handler);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnNotification(string method, Func<Task> handler) => this.OnNotification(method, handler);
        public override IRequestProcessIdentifier RequestProcessIdentifier { get; set; } = new RequestProcessIdentifier();
    }
}

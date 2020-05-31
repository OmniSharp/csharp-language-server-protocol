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
using Newtonsoft.Json.Linq;
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

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.AddHandler(string method, IJsonRpcHandler handler) => AddHandler(method, handler);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.AddHandler(string method, Func<IServiceProvider, IJsonRpcHandler> handlerFunc) => AddHandler(method, handlerFunc);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.AddHandlers(params IJsonRpcHandler[] handlers) => AddHandlers(handlers);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.AddHandler<THandler>(Func<IServiceProvider, THandler> handlerFunc) => AddHandler(handlerFunc);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.AddHandler<THandler>(THandler handler) => AddHandler(handler);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.AddHandler<TTHandler>() => AddHandler<TTHandler>();

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.AddHandler<TTHandler>(string method) => AddHandler<TTHandler>(method);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.AddHandler(Type type) => AddHandler(type);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.AddHandler(string method, Type type) => AddHandler(method, type);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnJsonRequest(string method, Func<JToken, Task<JToken>> handler) => OnJsonRequest(method, handler);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnJsonRequest(string method, Func<JToken, CancellationToken, Task<JToken>> handler) => OnJsonRequest(method, handler);
        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnRequest<TParams, TResponse>(string method, Func<TParams, Task<TResponse>> handler) => OnRequest(method, handler);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnRequest<TParams, TResponse>(string method, Func<TParams, CancellationToken, Task<TResponse>> handler) => OnRequest(method, handler);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnRequest<TResponse>(string method, Func<Task<TResponse>> handler) => OnRequest(method, handler);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnRequest<TResponse>(string method, Func<CancellationToken, Task<TResponse>> handler) => OnRequest(method, handler);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnRequest<TParams>(string method, Func<TParams, Task> handler) => OnRequest(method, handler);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnRequest<TParams>(string method, Func<TParams, CancellationToken, Task> handler) => OnRequest(method, handler);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnRequest<TParams>(string method, Func<CancellationToken, Task> handler) => OnRequest<TParams>(method, handler);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnNotification<TParams>(string method, Action<TParams, CancellationToken> handler) => OnNotification(method, handler);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnJsonNotification(string method, Action<JToken> handler) => OnJsonNotification(method, handler);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnJsonNotification(string method, Func<JToken, CancellationToken, Task> handler) => OnJsonNotification(method, handler);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnJsonNotification(string method, Func<JToken, Task> handler) => OnJsonNotification(method, handler);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnJsonNotification(string method, Action<JToken, CancellationToken> handler) => OnJsonNotification(method, handler);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnNotification<TParams>(string method, Action<TParams> handler) => OnNotification(method, handler);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnNotification<TParams>(string method, Func<TParams, CancellationToken, Task> handler) => OnNotification(method, handler);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnNotification<TParams>(string method, Func<TParams, Task> handler) => OnNotification(method, handler);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnNotification(string method, Action handler) => OnNotification(method, handler);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnNotification(string method, Func<CancellationToken, Task> handler) => OnNotification(method, handler);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnNotification(string method, Func<Task> handler) => OnNotification(method, handler);
        public override IRequestProcessIdentifier RequestProcessIdentifier { get; set; }
    }
}

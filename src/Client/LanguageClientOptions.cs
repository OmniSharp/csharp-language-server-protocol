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
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;
using OmniSharp.Extensions.LanguageServer.Shared;
using ISerializer = OmniSharp.Extensions.LanguageServer.Protocol.Serialization.ISerializer;

namespace OmniSharp.Extensions.LanguageServer.Client
{
    public class LanguageClientOptions : LanguageProtocolRpcOptionsBase<LanguageClientOptions>, ILanguageClientRegistry
    {
        public ClientCapabilities ClientCapabilities { get; set; } = new ClientCapabilities() {
            Experimental = new Dictionary<string, JToken>(),
            Window = new WindowClientCapabilities(),
            Workspace = new WorkspaceClientCapabilities(),
            TextDocument = new TextDocumentClientCapabilities()
        };

        public ClientInfo ClientInfo { get; set; }
        public DocumentUri RootUri { get; set; }
        public bool WorkspaceFolders { get; set; } = true;
        public bool DynamicRegistration { get; set; } = true;
        public bool ProgressTokens { get; set; } = true;
        internal List<WorkspaceFolder> Folders { get; set; } = new List<WorkspaceFolder>();

        public string RootPath
        {
            get => RootUri.GetFileSystemPath();
            set => RootUri = DocumentUri.FromFileSystemPath(value);
        }

        public InitializeTrace Trace { get; set; }

        public object InitializationOptions { get; set; }

        public ISerializer Serializer { get; set; } = new Protocol.Serialization.Serializer(ClientVersion.Lsp3);
        public ILspClientReceiver Receiver { get; set; } = new LspClientReceiver();
        internal List<ICapability> SupportedCapabilities { get; set; } = new List<ICapability>();

        internal readonly List<OnClientStartedDelegate> StartedDelegates = new List<OnClientStartedDelegate>();
        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.AddHandler(string method, IJsonRpcHandler handler) => this.AddHandler(method, handler);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.AddHandler(string method, Func<IServiceProvider, IJsonRpcHandler> handlerFunc) => this.AddHandler(method, handlerFunc);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.AddHandlers(params IJsonRpcHandler[] handlers) => this.AddHandlers(handlers);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.AddHandler<THandler>(Func<IServiceProvider, THandler> handlerFunc) => this.AddHandler(handlerFunc);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.AddHandler<THandler>(THandler handler) => this.AddHandler(handler);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.AddHandler<TTHandler>() => this.AddHandler<TTHandler>();

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.AddHandler<TTHandler>(string method) => this.AddHandler<TTHandler>(method);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.AddHandler(Type type) => this.AddHandler(type);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.AddHandler(string method, Type type) => this.AddHandler(method, type);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnRequest<TParams, TResponse>(string method, Func<TParams, Task<TResponse>> handler) => this.OnRequest(method, handler);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnRequest<TResponse>(string method, Func<Task<TResponse>> handler) => this.OnRequest(method, handler);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnRequest<TParams, TResponse>(string method, Func<TParams, CancellationToken, Task<TResponse>> handler) => this.OnRequest(method, handler);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnRequest<TResponse>(string method, Func<CancellationToken, Task<TResponse>> handler) => this.OnRequest(method, handler);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnRequest<TParams>(string method, Func<TParams, Task> handler) => this.OnRequest(method, handler);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnRequest<TParams>(string method, Func<TParams, CancellationToken, Task> handler) => this.OnRequest(method, handler);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnRequest<TParams>(string method, Func<CancellationToken, Task> handler) => this.OnRequest<TParams>(method, handler);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnNotification<TParams>(string method, Action<TParams, CancellationToken> handler) => this.OnNotification(method, handler);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnNotification(string method, Action handler) => this.OnNotification(method, handler);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnNotification<TParams>(string method, Action<TParams> handler) => this.OnNotification(method, handler);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnNotification<TParams>(string method, Func<TParams, CancellationToken, Task> handler) => this.OnNotification(method, handler);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnNotification<TParams>(string method, Func<TParams, Task> handler) => this.OnNotification(method, handler);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnNotification(string method, Func<CancellationToken, Task> handler) => this.OnNotification(method, handler);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnNotification(string method, Func<Task> handler) => this.OnNotification(method, handler);

        public override IRequestProcessIdentifier RequestProcessIdentifier { get; set; } = new RequestProcessIdentifier();
    }
}

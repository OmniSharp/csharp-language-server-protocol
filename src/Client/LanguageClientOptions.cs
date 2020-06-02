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
        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.AddHandler(string method, IJsonRpcHandler handler, JsonRpcHandlerOptions options) => this.AddHandler(method, handler, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.AddHandler(string method, Func<IServiceProvider, IJsonRpcHandler> handlerFunc, JsonRpcHandlerOptions options) => this.AddHandler(method, handlerFunc, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.AddHandlers(params IJsonRpcHandler[] handlers) => this.AddHandlers(handlers);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.AddHandler<THandler>(Func<IServiceProvider, THandler> handlerFunc, JsonRpcHandlerOptions options) => this.AddHandler(handlerFunc, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.AddHandler<THandler>(THandler handler, JsonRpcHandlerOptions options) => this.AddHandler(handler, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.AddHandler<TTHandler>(JsonRpcHandlerOptions options) => this.AddHandler<TTHandler>(options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.AddHandler<TTHandler>(string method, JsonRpcHandlerOptions options) => this.AddHandler<TTHandler>(method, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.AddHandler(Type type, JsonRpcHandlerOptions options) => this.AddHandler(type, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.AddHandler(string method, Type type, JsonRpcHandlerOptions options) => this.AddHandler(method, type, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnJsonRequest(string method, Func<JToken, Task<JToken>> handler, JsonRpcHandlerOptions options) => OnJsonRequest(method, handler, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnJsonRequest(string method, Func<JToken, CancellationToken, Task<JToken>> handler, JsonRpcHandlerOptions options) => OnJsonRequest(method, handler, options);
        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnRequest<TParams, TResponse>(string method, Func<TParams, Task<TResponse>> handler, JsonRpcHandlerOptions options) => OnRequest(method, handler, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnRequest<TParams, TResponse>(string method, Func<TParams, CancellationToken, Task<TResponse>> handler, JsonRpcHandlerOptions options) => OnRequest(method, handler, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnRequest<TResponse>(string method, Func<Task<TResponse>> handler, JsonRpcHandlerOptions options) => OnRequest(method, handler, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnRequest<TResponse>(string method, Func<CancellationToken, Task<TResponse>> handler, JsonRpcHandlerOptions options) => OnRequest(method, handler, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnRequest<TParams>(string method, Func<TParams, Task> handler, JsonRpcHandlerOptions options) => OnRequest(method, handler, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnRequest<TParams>(string method, Func<TParams, CancellationToken, Task> handler, JsonRpcHandlerOptions options) => OnRequest(method, handler, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnRequest<TParams>(string method, Func<CancellationToken, Task> handler, JsonRpcHandlerOptions options) => OnRequest<TParams>(method, handler, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnNotification<TParams>(string method, Action<TParams, CancellationToken> handler, JsonRpcHandlerOptions options) => OnNotification(method, handler, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnJsonNotification(string method, Action<JToken> handler, JsonRpcHandlerOptions options) => OnJsonNotification(method, handler, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnJsonNotification(string method, Func<JToken, CancellationToken, Task> handler, JsonRpcHandlerOptions options) => OnJsonNotification(method, handler, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnJsonNotification(string method, Func<JToken, Task> handler, JsonRpcHandlerOptions options) => OnJsonNotification(method, handler, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnJsonNotification(string method, Action<JToken, CancellationToken> handler, JsonRpcHandlerOptions options) => OnJsonNotification(method, handler, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnNotification<TParams>(string method, Action<TParams> handler, JsonRpcHandlerOptions options) => OnNotification(method, handler, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnNotification<TParams>(string method, Func<TParams, CancellationToken, Task> handler, JsonRpcHandlerOptions options) => OnNotification(method, handler, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnNotification<TParams>(string method, Func<TParams, Task> handler, JsonRpcHandlerOptions options) => OnNotification(method, handler, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnNotification(string method, Action handler, JsonRpcHandlerOptions options) => OnNotification(method, handler, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnNotification(string method, Func<CancellationToken, Task> handler, JsonRpcHandlerOptions options) => OnNotification(method, handler, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnNotification(string method, Func<Task> handler, JsonRpcHandlerOptions options) => OnNotification(method, handler, options);

        public override IRequestProcessIdentifier RequestProcessIdentifier { get; set; }
    }
}

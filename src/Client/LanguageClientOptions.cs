using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Shared;

namespace OmniSharp.Extensions.LanguageServer.Client
{
    public class LanguageClientOptions : LanguageProtocolRpcOptionsBase<LanguageClientOptions>, ILanguageClientRegistry
    {
        public LanguageClientOptions()
        {
            WithAssemblies(typeof(LanguageClientOptions).Assembly, typeof(LspRequestRouter).Assembly);
        }
        public ClientCapabilities ClientCapabilities { get; set; } = new ClientCapabilities {
            Experimental = new Dictionary<string, JToken>(),
            Window = new WindowClientCapabilities(),
            Workspace = new WorkspaceClientCapabilities(),
            TextDocument = new TextDocumentClientCapabilities()
        };

        public ClientInfo? ClientInfo { get; set; }
        public DocumentUri? RootUri { get; set; }
        public bool WorkspaceFolders { get; set; } = true;
        public bool DynamicRegistration { get; set; } = true;
        public bool ProgressTokens { get; set; } = true;

        public string RootPath
        {
            get => RootUri?.GetFileSystemPath() ?? string.Empty;
            set => RootUri = DocumentUri.FromFileSystemPath(value);
        }

        public InitializeTrace Trace { get; set; }

        public object? InitializationOptions { get; set; }

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.AddHandler(string method, IJsonRpcHandler handler, JsonRpcHandlerOptions? options) =>
            AddHandler(method, handler, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.AddHandler(string method, JsonRpcHandlerFactory handlerFunc, JsonRpcHandlerOptions? options) =>
            AddHandler(method, handlerFunc, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.AddHandlers(params IJsonRpcHandler[] handlers) => AddHandlers(handlers);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.AddHandler(JsonRpcHandlerFactory handlerFunc, JsonRpcHandlerOptions? options) =>
            AddHandler(handlerFunc, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.AddHandler(IJsonRpcHandler handler, JsonRpcHandlerOptions? options) => AddHandler(handler, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.AddHandler<TTHandler>(JsonRpcHandlerOptions? options) => AddHandler<TTHandler>(options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.AddHandler<TTHandler>(string method, JsonRpcHandlerOptions? options) =>
            AddHandler<TTHandler>(method, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.AddHandler(Type type, JsonRpcHandlerOptions? options) => AddHandler(type, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.AddHandler(string method, Type type, JsonRpcHandlerOptions? options) =>
            AddHandler(method, type, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.AddHandlerLink(string fromMethod, string toMethod) =>
            AddHandlerLink(fromMethod, toMethod);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnJsonRequest(string method, Func<JToken, Task<JToken>> handler, JsonRpcHandlerOptions? options) =>
            OnJsonRequest(method, handler, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnJsonRequest(
            string method, Func<JToken, CancellationToken, Task<JToken>> handler, JsonRpcHandlerOptions? options
        ) => OnJsonRequest(method, handler, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnRequest<TParams, TResponse>(
            string method, Func<TParams, Task<TResponse>> handler, JsonRpcHandlerOptions? options
        ) => OnRequest(method, handler, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnRequest<TParams, TResponse>(
            string method, Func<TParams, CancellationToken, Task<TResponse>> handler, JsonRpcHandlerOptions? options
        ) => OnRequest(method, handler, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.
            OnRequest<TResponse>(string method, Func<Task<TResponse>> handler, JsonRpcHandlerOptions? options) => OnRequest(method, handler, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnRequest<TResponse>(
            string method, Func<CancellationToken, Task<TResponse>> handler, JsonRpcHandlerOptions? options
        ) => OnRequest(method, handler, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnRequest<TParams>(string method, Func<TParams, Task> handler, JsonRpcHandlerOptions? options) =>
            OnRequest(method, handler, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnRequest<TParams>(
            string method, Func<TParams, CancellationToken, Task> handler, JsonRpcHandlerOptions? options
        ) => OnRequest(method, handler, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnRequest<TParams>(
            string method, Func<CancellationToken, Task> handler, JsonRpcHandlerOptions? options
        ) => OnRequest<TParams>(method, handler, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnNotification<TParams>(
            string method, Action<TParams, CancellationToken> handler, JsonRpcHandlerOptions? options
        ) => OnNotification(method, handler, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnJsonNotification(string method, Action<JToken> handler, JsonRpcHandlerOptions? options) =>
            OnJsonNotification(method, handler, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnJsonNotification(
            string method, Func<JToken, CancellationToken, Task> handler, JsonRpcHandlerOptions? options
        ) => OnJsonNotification(method, handler, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnJsonNotification(string method, Func<JToken, Task> handler, JsonRpcHandlerOptions? options) =>
            OnJsonNotification(method, handler, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnJsonNotification(
            string method, Action<JToken, CancellationToken> handler, JsonRpcHandlerOptions? options
        ) => OnJsonNotification(method, handler, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnNotification<TParams>(string method, Action<TParams> handler, JsonRpcHandlerOptions? options) =>
            OnNotification(method, handler, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnNotification<TParams>(
            string method, Func<TParams, CancellationToken, Task> handler, JsonRpcHandlerOptions? options
        ) => OnNotification(method, handler, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.
            OnNotification<TParams>(string method, Func<TParams, Task> handler, JsonRpcHandlerOptions? options) => OnNotification(method, handler, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnNotification(string method, Action handler, JsonRpcHandlerOptions? options) =>
            OnNotification(method, handler, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.
            OnNotification(string method, Func<CancellationToken, Task> handler, JsonRpcHandlerOptions? options) => OnNotification(method, handler, options);

        ILanguageClientRegistry IJsonRpcHandlerRegistry<ILanguageClientRegistry>.OnNotification(string method, Func<Task> handler, JsonRpcHandlerOptions? options) =>
            OnNotification(method, handler, options);
    }
}

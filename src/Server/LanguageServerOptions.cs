using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public class LanguageServerOptions : LanguageProtocolRpcOptionsBase<LanguageServerOptions>, ILanguageServerRegistry
    {
        public ServerInfo ServerInfo { get; set; }
        public ILspServerReceiver Receiver { get; set; } = new LspServerReceiver();

        public LanguageServerOptions WithReceiver(ILspServerReceiver receiver)
        {
            Receiver = receiver;
            return this;
        }

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.AddHandler(string method, IJsonRpcHandler handler, JsonRpcHandlerOptions options) =>
            AddHandler(method, handler, options);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.AddHandler(string method, JsonRpcHandlerFactory handlerFunc, JsonRpcHandlerOptions options) =>
            AddHandler(method, handlerFunc, options);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.AddHandlers(params IJsonRpcHandler[] handlers) => AddHandlers(handlers);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.AddHandler(JsonRpcHandlerFactory handlerFunc, JsonRpcHandlerOptions options) =>
            AddHandler(handlerFunc, options);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.AddHandler(IJsonRpcHandler handler, JsonRpcHandlerOptions options) => AddHandler(handler, options);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.AddHandler<TTHandler>(JsonRpcHandlerOptions options) => AddHandler<TTHandler>(options);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.AddHandler<TTHandler>(string method, JsonRpcHandlerOptions options) =>
            AddHandler<TTHandler>(method, options);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.AddHandler(Type type, JsonRpcHandlerOptions options) => AddHandler(type, options);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.AddHandler(string method, Type type, JsonRpcHandlerOptions options) =>
            AddHandler(method, type, options);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnJsonRequest(string method, Func<JToken, Task<JToken>> handler, JsonRpcHandlerOptions options) =>
            OnJsonRequest(method, handler, options);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnJsonRequest(
            string method, Func<JToken, CancellationToken, Task<JToken>> handler, JsonRpcHandlerOptions options
        ) => OnJsonRequest(method, handler, options);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnRequest<TParams, TResponse>(
            string method, Func<TParams, Task<TResponse>> handler, JsonRpcHandlerOptions options
        ) => OnRequest(method, handler, options);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnRequest<TParams, TResponse>(
            string method, Func<TParams, CancellationToken, Task<TResponse>> handler, JsonRpcHandlerOptions options
        ) => OnRequest(method, handler, options);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.
            OnRequest<TResponse>(string method, Func<Task<TResponse>> handler, JsonRpcHandlerOptions options) => OnRequest(method, handler, options);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnRequest<TResponse>(
            string method, Func<CancellationToken, Task<TResponse>> handler, JsonRpcHandlerOptions options
        ) => OnRequest(method, handler, options);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnRequest<TParams>(string method, Func<TParams, Task> handler, JsonRpcHandlerOptions options) =>
            OnRequest(method, handler, options);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnRequest<TParams>(
            string method, Func<TParams, CancellationToken, Task> handler, JsonRpcHandlerOptions options
        ) => OnRequest(method, handler, options);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnRequest<TParams>(
            string method, Func<CancellationToken, Task> handler, JsonRpcHandlerOptions options
        ) => OnRequest<TParams>(method, handler, options);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnNotification<TParams>(
            string method, Action<TParams, CancellationToken> handler, JsonRpcHandlerOptions options
        ) => OnNotification(method, handler, options);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnJsonNotification(string method, Action<JToken> handler, JsonRpcHandlerOptions options) =>
            OnJsonNotification(method, handler, options);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnJsonNotification(
            string method, Func<JToken, CancellationToken, Task> handler, JsonRpcHandlerOptions options
        ) => OnJsonNotification(method, handler, options);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnJsonNotification(string method, Func<JToken, Task> handler, JsonRpcHandlerOptions options) =>
            OnJsonNotification(method, handler, options);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnJsonNotification(
            string method, Action<JToken, CancellationToken> handler, JsonRpcHandlerOptions options
        ) => OnJsonNotification(method, handler, options);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnNotification<TParams>(string method, Action<TParams> handler, JsonRpcHandlerOptions options) =>
            OnNotification(method, handler, options);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnNotification<TParams>(
            string method, Func<TParams, CancellationToken, Task> handler, JsonRpcHandlerOptions options
        ) => OnNotification(method, handler, options);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.
            OnNotification<TParams>(string method, Func<TParams, Task> handler, JsonRpcHandlerOptions options) => OnNotification(method, handler, options);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnNotification(string method, Action handler, JsonRpcHandlerOptions options) =>
            OnNotification(method, handler, options);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.
            OnNotification(string method, Func<CancellationToken, Task> handler, JsonRpcHandlerOptions options) => OnNotification(method, handler, options);

        ILanguageServerRegistry IJsonRpcHandlerRegistry<ILanguageServerRegistry>.OnNotification(string method, Func<Task> handler, JsonRpcHandlerOptions options) =>
            OnNotification(method, handler, options);

        public override IRequestProcessIdentifier RequestProcessIdentifier { get; set; }
    }
}

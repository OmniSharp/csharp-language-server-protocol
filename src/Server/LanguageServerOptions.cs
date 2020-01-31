using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Disposables;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Server.Abstractions;
using ISerializer = OmniSharp.Extensions.LanguageServer.Protocol.Serialization.ISerializer;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public class LanguageServerOptions : ILanguageServerRegistry
    {
        public LanguageServerOptions()
        {
        }

        public Stream Input { get; set; }
        public Stream Output { get; set; }
        public ISerializer Serializer { get; set; } = Protocol.Serialization.Serializer.Instance;
        public IRequestProcessIdentifier RequestProcessIdentifier { get; set; } = new RequestProcessIdentifier();
        public ILspReciever Reciever { get; set; } = new LspReciever();
        public IServiceCollection Services { get; set; } = new ServiceCollection();
        internal List<IJsonRpcHandler> Handlers { get; set; } = new List<IJsonRpcHandler>();
        internal List<ITextDocumentIdentifier> TextDocumentIdentifiers { get; set; } = new List<ITextDocumentIdentifier>();
        internal List<(string name, IJsonRpcHandler handler)> NamedHandlers { get; set; } = new List<(string name, IJsonRpcHandler handler)>();
        internal List<(string name, Func<IServiceProvider, IJsonRpcHandler> handlerFunc)> NamedServiceHandlers { get; set; } = new List<(string name, Func<IServiceProvider, IJsonRpcHandler> handlerFunc)>();
        internal List<Type> HandlerTypes { get; set; } = new List<Type>();
        internal List<Type> TextDocumentIdentifierTypes { get; set; } = new List<Type>();
        internal List<Assembly> HandlerAssemblies { get; set; } = new List<Assembly>();
        internal Action<ILoggingBuilder> LoggingBuilderAction { get; set; } = new Action<ILoggingBuilder>(_ => { });

        internal readonly List<InitializeDelegate> InitializeDelegates = new List<InitializeDelegate>();
        internal readonly List<InitializedDelegate> InitializedDelegates = new List<InitializedDelegate>();
        internal readonly List<StartedDelegate> StartedDelegates = new List<StartedDelegate>();

        public IDisposable AddHandler(string method, IJsonRpcHandler handler)
        {
            NamedHandlers.Add((method, handler));
            return Disposable.Empty;
        }

        public IDisposable AddHandler(string method, Func<IServiceProvider, IJsonRpcHandler> handlerFunc)
        {
            NamedServiceHandlers.Add((method, handlerFunc));
            return Disposable.Empty;
        }

        public IDisposable AddHandlers(params IJsonRpcHandler[] handlers)
        {
            Handlers.AddRange(handlers);
            return Disposable.Empty;
        }

        public IDisposable AddHandler<T>() where T : IJsonRpcHandler
        {
            HandlerTypes.Add(typeof(T));
            return Disposable.Empty;
        }

        public IDisposable AddTextDocumentIdentifier(params ITextDocumentIdentifier[] handlers)
        {
            TextDocumentIdentifiers.AddRange(handlers);
            return Disposable.Empty;
        }

        public IDisposable AddTextDocumentIdentifier<T>() where T : ITextDocumentIdentifier
        {
            TextDocumentIdentifierTypes.Add(typeof(T));
            return Disposable.Empty;
        }
    }
}

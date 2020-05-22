using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Reactive.Disposables;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;
using OmniSharp.Extensions.LanguageServer.Shared;
using ISerializer = OmniSharp.Extensions.LanguageServer.Protocol.Serialization.ISerializer;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public class LanguageServerOptions : ILanguageServerRegistry, IJsonRpcServerOptions
    {
        public LanguageServerOptions()
        {
        }

        public PipeReader Input { get; set; }
        public PipeWriter Output { get; set; }
        public ServerInfo ServerInfo { get; set; }
        public ISerializer Serializer { get; set; } = new Protocol.Serialization.Serializer(ClientVersion.Lsp3);
        public IRequestProcessIdentifier RequestProcessIdentifier { get; set; } = new RequestProcessIdentifier();
        public ILspServerReceiver Receiver { get; set; } = new LspServerReceiver();
        public IServiceCollection Services { get; set; } = new ServiceCollection();
        internal List<IJsonRpcHandler> Handlers { get; set; } = new List<IJsonRpcHandler>();
        internal List<ITextDocumentIdentifier> TextDocumentIdentifiers { get; set; } = new List<ITextDocumentIdentifier>();
        internal List<(string name, IJsonRpcHandler handler)> NamedHandlers { get; set; } = new List<(string name, IJsonRpcHandler handler)>();
        internal List<(string name, Func<IServiceProvider, IJsonRpcHandler> handlerFunc)> NamedServiceHandlers { get; set; } = new List<(string name, Func<IServiceProvider, IJsonRpcHandler> handlerFunc)>();
        internal List<Type> HandlerTypes { get; set; } = new List<Type>();
        internal List<Type> TextDocumentIdentifierTypes { get; set; } = new List<Type>();
        internal List<Assembly> HandlerAssemblies { get; set; } = new List<Assembly>();
        internal Action<ILoggingBuilder> LoggingBuilderAction { get; set; } = new Action<ILoggingBuilder>(_ => { });
        internal Action<IConfigurationBuilder> ConfigurationBuilderAction { get; set; } = new Action<IConfigurationBuilder>(_ => { });
        internal bool AddDefaultLoggingProvider { get; set; }
        public int? Concurrency { get; set; }
        public Func<ServerError, IHandlerDescriptor, Exception> OnServerError { get; set; }
        public bool SupportsContentModified { get; set; } = true;
        public TimeSpan MaximumRequestTimeout { get; set; } = TimeSpan.FromMinutes(5);

        internal readonly List<InitializeDelegate> InitializeDelegates = new List<InitializeDelegate>();
        internal readonly List<InitializedDelegate> InitializedDelegates = new List<InitializedDelegate>();
        internal readonly List<OnServerStartedDelegate> StartedDelegates = new List<OnServerStartedDelegate>();

        internal CompositeDisposable CompositeDisposable { get; } = new CompositeDisposable();

        public void RegisterForDisposal(IDisposable disposable)
        {
            CompositeDisposable.Add(disposable);
        }

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

        public IDisposable AddHandler<T>(Func<IServiceProvider, T> handlerFunc) where T : IJsonRpcHandler
        {
            NamedServiceHandlers.Add((HandlerTypeDescriptorHelper.GetMethodName<T>(), _ => handlerFunc(_)));
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

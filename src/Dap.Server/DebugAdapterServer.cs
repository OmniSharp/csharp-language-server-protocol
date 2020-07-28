using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Server
{
    public class DebugAdapterServer : JsonRpcServerBase, IDebugAdapterServer
    {
        public static Task<IDebugAdapterServer> From(Action<DebugAdapterServerOptions> optionsAction)
        {
            return From(optionsAction, CancellationToken.None);
        }

        public static Task<IDebugAdapterServer> From(DebugAdapterServerOptions options)
        {
            return From(options, CancellationToken.None);
        }

        public static Task<IDebugAdapterServer> From(Action<DebugAdapterServerOptions> optionsAction, CancellationToken token)
        {
            var options = new DebugAdapterServerOptions();
            optionsAction(options);
            return From(options, token);
        }

        public static IDebugAdapterServer PreInit(Action<DebugAdapterServerOptions> optionsAction)
        {
            var options = new DebugAdapterServerOptions();
            optionsAction(options);
            return PreInit(options);
        }

        public static async Task<IDebugAdapterServer> From(DebugAdapterServerOptions options, CancellationToken token)
        {
            var server = (DebugAdapterServer)PreInit(options);
            await server.Initialize(token);

            return server;
        }

        /// <summary>
        /// Create the server without connecting to the client
        ///
        /// Mainly used for unit testing
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IDebugAdapterServer PreInit(DebugAdapterServerOptions options)
        {
            return new DebugAdapterServer(options);
        }

        internal DebugAdapterServer(DebugAdapterServerOptions options) : base(options)
        {
        }

        protected override IResponseRouter ResponseRouter { get; }
        protected override IHandlersManager HandlersManager { get; }
    }

    public class DebugAdapterServerOptions : DebugAdapterRpcOptionsBase<DebugAdapterServerOptions>, IDebugAdapterServerRegistry
    {
        public Capabilities Capabilities { get; set; } = new Capabilities();
        internal readonly List<OnClientStartedDelegate> StartedDelegates = new List<OnClientStartedDelegate>();
        internal readonly List<InitializedDelegate> InitializedDelegates = new List<InitializedDelegate>();
        internal readonly List<InitializeDelegate> InitializeDelegates = new List<InitializeDelegate>();
        public ISerializer Serializer { get; set; } = new DapSerializer();
        public override IRequestProcessIdentifier RequestProcessIdentifier { get; set; } = new ParallelRequestProcessIdentifier();
        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.AddHandler(string method, IJsonRpcHandler handler, JsonRpcHandlerOptions options) => this.AddHandler(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.AddHandler(string method, Func<IServiceProvider, IJsonRpcHandler> handlerFunc, JsonRpcHandlerOptions options) => this.AddHandler(method, handlerFunc, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.AddHandlers(params IJsonRpcHandler[] handlers) => this.AddHandlers(handlers);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.AddHandler<THandler>(Func<IServiceProvider, THandler> handlerFunc, JsonRpcHandlerOptions options) => this.AddHandler(handlerFunc, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.AddHandler<THandler>(THandler handler, JsonRpcHandlerOptions options) => this.AddHandler(handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.AddHandler<TTHandler>(JsonRpcHandlerOptions options) => this.AddHandler<TTHandler>(options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.AddHandler<TTHandler>(string method, JsonRpcHandlerOptions options) => this.AddHandler<TTHandler>(method, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.AddHandler(Type type, JsonRpcHandlerOptions options) => this.AddHandler(type, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.AddHandler(string method, Type type, JsonRpcHandlerOptions options) => this.AddHandler(method, type, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnJsonRequest(string method, Func<JToken, Task<JToken>> handler, JsonRpcHandlerOptions options) => OnJsonRequest(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnJsonRequest(string method, Func<JToken, CancellationToken, Task<JToken>> handler, JsonRpcHandlerOptions options) => OnJsonRequest(method, handler, options);
        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnRequest<TParams, TResponse>(string method, Func<TParams, Task<TResponse>> handler, JsonRpcHandlerOptions options) => OnRequest(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnRequest<TParams, TResponse>(string method, Func<TParams, CancellationToken, Task<TResponse>> handler, JsonRpcHandlerOptions options) => OnRequest(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnRequest<TResponse>(string method, Func<Task<TResponse>> handler, JsonRpcHandlerOptions options) => OnRequest(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnRequest<TResponse>(string method, Func<CancellationToken, Task<TResponse>> handler, JsonRpcHandlerOptions options) => OnRequest(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnRequest<TParams>(string method, Func<TParams, Task> handler, JsonRpcHandlerOptions options) => OnRequest(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnRequest<TParams>(string method, Func<TParams, CancellationToken, Task> handler, JsonRpcHandlerOptions options) => OnRequest(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnRequest<TParams>(string method, Func<CancellationToken, Task> handler, JsonRpcHandlerOptions options) => OnRequest<TParams>(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnNotification<TParams>(string method, Action<TParams, CancellationToken> handler, JsonRpcHandlerOptions options) => OnNotification(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnJsonNotification(string method, Action<JToken> handler, JsonRpcHandlerOptions options) => OnJsonNotification(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnJsonNotification(string method, Func<JToken, CancellationToken, Task> handler, JsonRpcHandlerOptions options) => OnJsonNotification(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnJsonNotification(string method, Func<JToken, Task> handler, JsonRpcHandlerOptions options) => OnJsonNotification(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnJsonNotification(string method, Action<JToken, CancellationToken> handler, JsonRpcHandlerOptions options) => OnJsonNotification(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnNotification<TParams>(string method, Action<TParams> handler, JsonRpcHandlerOptions options) => OnNotification(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnNotification<TParams>(string method, Func<TParams, CancellationToken, Task> handler, JsonRpcHandlerOptions options) => OnNotification(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnNotification<TParams>(string method, Func<TParams, Task> handler, JsonRpcHandlerOptions options) => OnNotification(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnNotification(string method, Action handler, JsonRpcHandlerOptions options) => OnNotification(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnNotification(string method, Func<CancellationToken, Task> handler, JsonRpcHandlerOptions options) => OnNotification(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnNotification(string method, Func<Task> handler, JsonRpcHandlerOptions options) => OnNotification(method, handler, options);
    }

    public delegate Task InitializedDelegate(IDebugAdapterServer server, InitializeRequestArguments request, InitializeResponse response, CancellationToken cancellationToken);
    public delegate Task InitializeDelegate(IDebugAdapterServer server, InitializeRequestArguments request, CancellationToken cancellationToken);

    public static class DebugAdapterServerOptionsExtensions
    {

        public static DebugAdapterServerOptions WithSerializer(this DebugAdapterServerOptions options, ISerializer serializer)
        {
            options.Serializer = serializer;
            return options;
        }

        public static DebugAdapterServerOptions WithRequestProcessIdentifier(this DebugAdapterServerOptions options, IRequestProcessIdentifier requestProcessIdentifier)
        {
            options.RequestProcessIdentifier = requestProcessIdentifier;
            return options;
        }

        public static DebugAdapterServerOptions WithServices(this DebugAdapterServerOptions options, Action<IServiceCollection> servicesAction)
        {
            servicesAction(options.Services);
            return options;
        }

        public static DebugAdapterServerOptions OnInitialize(this DebugAdapterServerOptions options, InitializeDelegate @delegate)
        {
            options.InitializeDelegates.Add(@delegate);
            return options;
        }


        public static DebugAdapterServerOptions OnInitialized(this DebugAdapterServerOptions options, InitializedDelegate @delegate)
        {
            options.InitializedDelegates.Add(@delegate);
            return options;
        }

        public static DebugAdapterServerOptions OnStarted(this DebugAdapterServerOptions options,
            OnClientStartedDelegate @delegate)
        {
            options.StartedDelegates.Add(@delegate);
            return options;
        }

        public static DebugAdapterServerOptions ConfigureLogging(this DebugAdapterServerOptions options,
            Action<ILoggingBuilder> builderAction)
        {
            options.LoggingBuilderAction = builderAction;
            return options;
        }

        public static DebugAdapterServerOptions AddDefaultLoggingProvider(this DebugAdapterServerOptions options)
        {
            options.AddDefaultLoggingProvider = true;
            return options;
        }

        public static DebugAdapterServerOptions ConfigureConfiguration(this DebugAdapterServerOptions options,
            Action<IConfigurationBuilder> builderAction)
        {
            options.ConfigurationBuilderAction = builderAction;
            return options;
        }
    }

}

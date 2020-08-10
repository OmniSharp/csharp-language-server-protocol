using System;
using System.IO.Pipelines;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc.Testing;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Server;
using RealLanguageServer = OmniSharp.Extensions.LanguageServer.Server.LanguageServer;

namespace OmniSharp.Extensions.LanguageProtocol.Testing
{
    /// <summary>
    /// This is a test class that is designed to allow you configure an in memory lsp client and server to do testing of handlers or behaviors.
    /// </summary>
    public abstract class LanguageProtocolTestBase : JsonRpcTestBase
    {
        private ILanguageClient _client;
        private ILanguageServer _server;

        public LanguageProtocolTestBase(JsonRpcTestOptions testOptions) : base(testOptions)
        {
        }

        protected virtual void ConfigureClientInputOutput(PipeReader serverOutput, PipeWriter clientInput, LanguageClientOptions options)
        {
            options.WithInput(serverOutput).WithOutput(clientInput);
        }

        protected virtual void ConfigureServerInputOutput(PipeReader clientOutput, PipeWriter serverInput, LanguageServerOptions options)
        {
            options.WithInput(clientOutput).WithOutput(serverInput);
        }

        protected internal virtual (ILanguageClient client, ILanguageServer server) Create(
            Action<LanguageClientOptions> clientOptionsAction,
            Action<LanguageServerOptions> serverOptionsAction)
        {
            var clientPipe = new Pipe(TestOptions.DefaultPipeOptions);
            var serverPipe = new Pipe(TestOptions.DefaultPipeOptions);

            _client = LanguageClient.PreInit(options => {
                options
                    .WithLoggerFactory(TestOptions.ClientLoggerFactory)
                    .ConfigureLogging(x => x.SetMinimumLevel(LogLevel.Trace))
                    .Services
                    .AddTransient(typeof(IPipelineBehavior<,>), typeof(SettlePipeline<,>))
                    .AddSingleton(ClientEvents as IRequestSettler);
                ConfigureClientInputOutput(serverPipe.Reader, clientPipe.Writer, options);
                clientOptionsAction(options);
            });

            _server = RealLanguageServer.PreInit(options => {
                options
                    .WithLoggerFactory(TestOptions.ClientLoggerFactory)
                    .ConfigureLogging(x => x.SetMinimumLevel(LogLevel.Trace))
                    .Services
                    .AddTransient(typeof(IPipelineBehavior<,>), typeof(SettlePipeline<,>))
                    .AddSingleton(ServerEvents as IRequestSettler);
                ConfigureServerInputOutput(clientPipe.Reader, serverPipe.Writer, options);
                serverOptionsAction(options);
            });

            Disposable.Add(_client);
            Disposable.Add(_server);

            return (_client, _server);
        }

        protected internal virtual async Task<(ILanguageClient client, ILanguageServer server)> Initialize(
            Action<LanguageClientOptions> clientOptionsAction,
            Action<LanguageServerOptions> serverOptionsAction)
        {
            (_client, _server) = Create(clientOptionsAction, serverOptionsAction);

            return await ObservableEx.ForkJoin(
                Observable.FromAsync(_client.Initialize),
                Observable.FromAsync(_server.Initialize),
                (a, b) => (_client, _server)
            ).ToTask(CancellationToken);
        }

        protected virtual async Task<(ILanguageClient client, ILanguageServer server, TestConfigurationProvider configurationProvider)> InitializeWithConfiguration(
            Action<LanguageClientOptions> clientOptionsAction,
            Action<LanguageServerOptions> serverOptionsAction
        ) {
            var (client, server) = Create(options => {
                clientOptionsAction?.Invoke(options);
                options.WithCapability(new DidChangeConfigurationCapability());
                options.Services.AddSingleton<TestConfigurationProvider>();
            }, serverOptionsAction);

            return await ObservableEx.ForkJoin(
                Observable.FromAsync(client.Initialize),
                Observable.FromAsync(server.Initialize),
                (a, b) => (client, server, client.GetRequiredService<TestConfigurationProvider>())
            ).ToTask(CancellationToken);
        }
    }
}

using System;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc.Testing;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
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

        protected virtual void ConfigureClientInputOutput(PipeReader inMemoryReader, PipeWriter inMemoryWriter, LanguageClientOptions options)
        {
            options.WithInput(inMemoryReader).WithOutput(inMemoryWriter);
        }

        protected virtual void ConfigureServerInputOutput(PipeReader inMemoryReader, PipeWriter inMemoryWriter, LanguageServerOptions options)
        {
            options.WithInput(inMemoryReader).WithOutput(inMemoryWriter);
        }

        protected virtual async Task<(ILanguageClient client, ILanguageServer server)> Initialize(
            Action<LanguageClientOptions> clientOptionsAction,
            Action<LanguageServerOptions> serverOptionsAction)
        {
            var clientPipe = new Pipe();
            var serverPipe = new Pipe();

            _client = LanguageClient.PreInit(options => {
                options
                    .ConfigureLogging(x => {
                        x.SetMinimumLevel(LogLevel.Trace);
                        x.Services.AddSingleton(TestOptions.ClientLoggerFactory);
                    })
                    .Services
                    .AddTransient(typeof(IPipelineBehavior<,>), typeof(SettlePipeline<,>))
                    .AddSingleton(ServerEvents as IRequestSettler);
                ConfigureClientInputOutput(serverPipe.Reader, clientPipe.Writer, options);
                clientOptionsAction(options);
            });

            _server = RealLanguageServer.PreInit(options => {
                options
                    .ConfigureLogging(x => {
                        x.SetMinimumLevel(LogLevel.Trace);
                        x.Services.AddSingleton(TestOptions.ServerLoggerFactory);
                    })
                    .Services
                    .AddTransient(typeof(IPipelineBehavior<,>), typeof(SettlePipeline<,>))
                    .AddSingleton(ServerEvents as IRequestSettler);
                ConfigureServerInputOutput(clientPipe.Reader, serverPipe.Writer, options);
                serverOptionsAction(options);
            });

            Disposable.Add(_client);
            Disposable.Add(_server);

            return await ObservableEx.ForkJoin(
                Observable.FromAsync(_client.Initialize),
                Observable.FromAsync(_server.Initialize),
                (a, b) => (_client, _server)
            ).ToTask(CancellationToken);
        }
    }
}

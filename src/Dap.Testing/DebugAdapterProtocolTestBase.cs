using System;
using System.IO.Pipelines;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.DebugAdapter.Client;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Client;
using OmniSharp.Extensions.DebugAdapter.Protocol.Server;
using OmniSharp.Extensions.DebugAdapter.Server;
using OmniSharp.Extensions.JsonRpc.Testing;

namespace OmniSharp.Extensions.DebugAdapter.Testing
{
    /// <summary>
    /// This is a test class that is designed to allow you configure an in memory lsp client and server to do testing of handlers or behaviors.
    /// </summary>
    public abstract class DebugAdapterProtocolTestBase : JsonRpcTestBase
    {
        private IDebugAdapterClient _client;
        private IDebugAdapterServer _server;

        public DebugAdapterProtocolTestBase(JsonRpcTestOptions testOptions) : base(testOptions)
        {
        }

        protected virtual void ConfigureClientInputOutput(PipeReader serverOutput, PipeWriter clientInput, DebugAdapterClientOptions options)
        {
            options.WithInput(serverOutput).WithOutput(clientInput);
        }

        protected virtual void ConfigureServerInputOutput(PipeReader clientOutput, PipeWriter serverInput, DebugAdapterServerOptions options)
        {
            options.WithInput(clientOutput).WithOutput(serverInput);
        }

        protected virtual async Task<(IDebugAdapterClient client, IDebugAdapterServer server)> Initialize(
            Action<DebugAdapterClientOptions> clientOptionsAction,
            Action<DebugAdapterServerOptions> serverOptionsAction)
        {
            var clientPipe = new Pipe(TestOptions.DefaultPipeOptions);
            var serverPipe = new Pipe(TestOptions.DefaultPipeOptions);

            _client = DebugAdapterClient.Create(options => {
                options
                    .WithLoggerFactory(TestOptions.ClientLoggerFactory)
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

            _server = DebugAdapterServer.Create(options => {
                options
                    .WithLoggerFactory(TestOptions.ServerLoggerFactory)
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

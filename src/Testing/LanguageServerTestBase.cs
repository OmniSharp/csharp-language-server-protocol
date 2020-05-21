using System;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc.Testing;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Server;

namespace OmniSharp.Extensions.LanguageProtocol.Testing
{
    /// <summary>
    /// This is a test class that is designed to allow you configure an in memory lsp client and and your server configuration to do integration tests against a server
    /// </summary>
    public abstract class LanguageServerTestBase : JsonRpcTestBase
    {
        private ILanguageClient _client;
        private ILanguageServer _server;

        public LanguageServerTestBase(JsonRpcTestOptions jsonRpcTestOptions) : base(jsonRpcTestOptions)
        {
        }

        protected abstract void ConfigureServer(LanguageServerOptions options);

        protected virtual async Task<ILanguageClient> InitializeClient(Action<LanguageClientOptions> clientOptionsAction =null)
        {
            var clientPipe = new Pipe();
            var serverPipe = new Pipe();

            _client = LanguageClient.PreInit(options => {
                options
                    .WithInput(serverPipe.Reader)
                    .WithOutput(clientPipe.Writer)
                    .ConfigureLogging(x => {
                        x.SetMinimumLevel(LogLevel.Trace);
                        x.Services.AddSingleton(TestOptions.ClientLoggerFactory);
                    })
                    .Services
                    .AddTransient(typeof(IPipelineBehavior<,>), typeof(SettlePipeline<,>))
                    .AddSingleton(ServerEvents as IRequestSettler);
                clientOptionsAction?.Invoke(options);
            });

            _server = LanguageServer.Server.LanguageServer.PreInit(options => {
                options
                    .ConfigureLogging(x => {
                        x.SetMinimumLevel(LogLevel.Trace);
                        x.Services.AddSingleton(TestOptions.ServerLoggerFactory);
                    })
                    .Services
                    .AddTransient(typeof(IPipelineBehavior<,>), typeof(SettlePipeline<,>))
                    .AddSingleton(ServerEvents as IRequestSettler);
                ConfigureServer(options);
            });

            Disposable.Add(_client);
            Disposable.Add(_server);

            return await ObservableEx.ForkJoin(
                Observable.FromAsync(_client.Initialize),
                Observable.FromAsync(_server.Initialize),
                (a, b) => _client
            ).ToTask(CancellationToken);
        }

        protected ILanguageServer GetServer()
        {
            if (_server == null)
            {
                throw new NotSupportedException("InitializeClient must have happened already");
            }

            return _server;
        }
    }
}

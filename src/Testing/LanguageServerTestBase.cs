using System;
using System.IO;
using System.IO.Pipelines;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
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

        public LanguageServerTestBase(JsonRpcTestOptions jsonRpcTestOptions) : base(jsonRpcTestOptions)
        {
        }

        protected abstract (Stream clientOutput, Stream serverInput) SetupServer();

        protected virtual async Task<ILanguageClient> InitializeClient(Action<LanguageClientOptions> clientOptionsAction = null)
        {
            _client = LanguageClient.PreInit(options => {
                var (reader, writer) = SetupServer();
                options
                    .WithInput(reader)
                    .WithOutput(writer)
                    .ConfigureLogging(x => {
                        x.SetMinimumLevel(LogLevel.Trace);
                        x.Services.AddSingleton(TestOptions.ClientLoggerFactory);
                    })
                    .Services
                    .AddTransient(typeof(IPipelineBehavior<,>), typeof(SettlePipeline<,>))
                    .AddSingleton(ServerEvents as IRequestSettler);
                clientOptionsAction?.Invoke(options);
            });

            Disposable.Add(_client);

            await _client.Initialize(CancellationToken);

            return _client;
        }
    }
}

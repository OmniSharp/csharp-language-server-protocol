using System;
using System.IO;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.DebugAdapter.Client;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Client;
using OmniSharp.Extensions.JsonRpc.Testing;

namespace OmniSharp.Extensions.DebugAdapter.Testing
{
    /// <summary>
    /// This is a test class that is designed to allow you configure an in memory lsp client and and your server configuration to do integration tests against a server
    /// </summary>
    public abstract class LanguageServerTestBase : JsonRpcTestBase
    {
        private IDebugAdapterClient _client;

        public LanguageServerTestBase(JsonRpcTestOptions jsonRpcTestOptions) : base(jsonRpcTestOptions)
        {
        }

        protected abstract (Stream clientOutput, Stream serverInput) SetupServer();

        protected virtual async Task<IDebugAdapterClient> InitializeClient(Action<DebugAdapterClientOptions> clientOptionsAction = null)
        {
            _client = DebugAdapterClient.Create(options => {
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

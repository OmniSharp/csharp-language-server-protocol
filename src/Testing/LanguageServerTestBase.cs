using System;
using System.IO;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc.Testing;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;

namespace OmniSharp.Extensions.LanguageProtocol.Testing
{
    /// <summary>
    /// This is a test class that is designed to allow you configure an in memory lsp client and and your server configuration to do integration tests against a server
    /// </summary>
    public abstract class LanguageServerTestBase : JsonRpcIntegrationServerTestBase
    {
        private ILanguageClient? _client;

        public LanguageServerTestBase(JsonRpcTestOptions jsonRpcTestOptions) : base(jsonRpcTestOptions)
        {
        }

        protected abstract (Stream clientOutput, Stream serverInput) SetupServer();

        protected virtual ILanguageClient CreateClient(Action<LanguageClientOptions>? clientOptionsAction = null)
        {
            _client = LanguageClient.PreInit(
                options => {
                    var (reader, writer) = SetupServer();
                    options
                       .WithInput(reader)
                       .WithOutput(writer)
                       .WithLoggerFactory(TestOptions.ClientLoggerFactory)
                       .WithAssemblies(TestOptions.Assemblies)
                       .WithAssemblies(typeof(LanguageProtocolTestBase).Assembly, GetType().Assembly)
                       .ConfigureLogging(x => x.SetMinimumLevel(LogLevel.Trace))
                       .Services
                       .AddTransient(typeof(IPipelineBehavior<,>), typeof(SettlePipeline<,>))
                       .AddSingleton(Events as IRequestSettler);
                    clientOptionsAction?.Invoke(options);
                }
            );

            Disposable.Add(_client);

            return _client;
        }

        protected virtual async Task<ILanguageClient> InitializeClient(Action<LanguageClientOptions>? clientOptionsAction = null)
        {
            _client = CreateClient(clientOptionsAction);
            await _client.Initialize(CancellationToken);

            return _client;
        }

        protected virtual async Task<(ILanguageClient client, TestConfigurationProvider configurationProvider)> InitializeClientWithConfiguration(
            Action<LanguageClientOptions>? clientOptionsAction = null
        )
        {
            var client = CreateClient(
                options => {
                    clientOptionsAction?.Invoke(options);
                    options.WithCapability(new DidChangeConfigurationCapability());
                    options.Services.AddSingleton<TestConfigurationProvider>();
                }
            );

            await client.Initialize(CancellationToken);

            return ( client, client.GetRequiredService<TestConfigurationProvider>() );
        }
    }
}

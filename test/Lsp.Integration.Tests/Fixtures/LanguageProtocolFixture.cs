using System.Threading.Tasks;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc.Testing;
using OmniSharp.Extensions.LanguageProtocol.Testing;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Xunit;
using Xunit.Abstractions;

namespace Lsp.Integration.Tests.Fixtures
{
    public class LanguageProtocolFixture<TConfigureFixture, TConfigureClient, TConfigureServer> : LanguageProtocolTestBase, IAsyncLifetime
        where TConfigureFixture : IConfigureLanguageProtocolFixture, new()
        where TConfigureClient : IConfigureLanguageClientOptions, new()
        where TConfigureServer : IConfigureLanguageServerOptions, new()
    {
        private readonly TestLoggerFactory _loggerServerFactory;
        private readonly TestLoggerFactory _loggerClientFactory;

        public LanguageProtocolFixture() :
            base(
                new TConfigureFixture().Configure(
                    new JsonRpcTestOptions(
                        new TestLoggerFactory(null, "{Timestamp:yyyy-MM-dd HH:mm:ss} [Server] [{Level}] {Message}{NewLine}{Exception}"),
                        new TestLoggerFactory(null, "{Timestamp:yyyy-MM-dd HH:mm:ss} [Client] [{Level}] {Message}{NewLine}{Exception}")
                    )
                )
            )
        {
            _loggerServerFactory = ( TestOptions.ServerLoggerFactory as TestLoggerFactory )!;
            _loggerClientFactory = ( TestOptions.ClientLoggerFactory as TestLoggerFactory )!;
        }

        public void Swap(ITestOutputHelper testOutputHelper)
        {
            _loggerServerFactory.Swap(testOutputHelper);
            _loggerClientFactory.Swap(testOutputHelper);
        }

        public ILanguageClient Client { get; private set; } = null!;
        public ILanguageServer Server { get; private set; } = null!;

        public async Task InitializeAsync()
        {
            var (client, server) = await Initialize(new TConfigureClient().Configure, new TConfigureServer().Configure);
            Client = client;
            Server = server;
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }
    }
}

using System.Threading.Tasks;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc.Testing;
using OmniSharp.Extensions.LanguageProtocol.Testing;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Xunit;
using Xunit.Abstractions;

namespace Lsp.Tests.Integration.Fixtures
{
    public class LanguageProtocolFixture<TConfigureFixture, TConfigureClient, TConfigureServer> : LanguageProtocolTestBase, IAsyncLifetime
        where TConfigureFixture : IConfigureLanguageProtocolFixture, new()
        where TConfigureClient : IConfigureLanguageClientOptions, new()
        where TConfigureServer : IConfigureLanguageServerOptions, new()
    {
        private readonly TestLoggerFactory _loggerFactory;

        public LanguageProtocolFixture() :
            base(new TConfigureFixture().Configure(new JsonRpcTestOptions(new TestLoggerFactory(null))))
        {
            _loggerFactory = TestOptions.ServerLoggerFactory as TestLoggerFactory;
        }

        public void Swap(ITestOutputHelper testOutputHelper)
        {
            _loggerFactory.Swap(testOutputHelper);
        }

        public ILanguageClient Client { get; private set; }
        public ILanguageServer Server { get; private set; }

        public async Task InitializeAsync()
        {
            var (client, server) = await Initialize(new TConfigureClient().Configure, new TConfigureServer().Configure);
            Client = client;
            Server = server;
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}

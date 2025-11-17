using System.Reactive;
using OmniSharp.Extensions.JsonRpc.Testing;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Xunit.Abstractions;

namespace Lsp.Integration.Tests.Fixtures
{
    public abstract class LanguageProtocolFixtureTest<TConfigureFixture, TConfigureClient, TConfigureServer>
        : IClassFixture<LanguageProtocolFixture<TConfigureFixture, TConfigureClient, TConfigureServer>>
        where TConfigureFixture : IConfigureLanguageProtocolFixture, new()
        where TConfigureClient : IConfigureLanguageClientOptions, new()
        where TConfigureServer : IConfigureLanguageServerOptions, new()
    {
        protected LanguageProtocolFixture<TConfigureFixture, TConfigureClient, TConfigureServer> Fixture { get; }

        protected LanguageProtocolFixtureTest(
            ITestOutputHelper testOutputHelper, LanguageProtocolFixture<TConfigureFixture, TConfigureClient, TConfigureServer> fixture
        )
        {
            Fixture = fixture;
            Client = fixture.Client;
            Server = fixture.Server;
            CancellationToken = fixture.CancellationToken;
            ClientEvents = fixture.ClientEvents;
            ServerEvents = fixture.ServerEvents;
            Events = fixture.Events;
            TestOptions = fixture.TestOptions;
            fixture.Swap(testOutputHelper);
        }

        protected JsonRpcTestOptions TestOptions { get; }
        protected ILanguageServer Server { get; }
        protected ILanguageClient Client { get; }
        protected CancellationToken CancellationToken { get; }
        protected ISettler ClientEvents { get; }
        protected ISettler ServerEvents { get; }
        protected ISettler Events { get; }

        protected Task SettleNext()
        {
            return Events.SettleNext();
        }

        protected IObservable<Unit> Settle()
        {
            return Events.Settle();
        }
    }
}

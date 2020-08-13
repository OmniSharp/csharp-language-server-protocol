using System;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.DebugAdapter.Protocol.Client;
using OmniSharp.Extensions.DebugAdapter.Protocol.Server;
using OmniSharp.Extensions.JsonRpc.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Dap.Tests.Integration.Fixtures
{
    public abstract class DebugAdapterProtocolFixtureTest<TConfigureFixture, TConfigureClient, TConfigureServer>
        : IClassFixture<DebugAdapterProtocolFixture<TConfigureFixture, TConfigureClient, TConfigureServer>>
        where TConfigureFixture : IConfigureDebugAdapterProtocolFixture, new()
        where TConfigureClient : IConfigureDebugAdapterClientOptions, new()
        where TConfigureServer : IConfigureDebugAdapterServerOptions, new()
    {
        protected DebugAdapterProtocolFixture<TConfigureFixture, TConfigureClient, TConfigureServer> Fixture { get; }

        protected DebugAdapterProtocolFixtureTest(ITestOutputHelper testOutputHelper, DebugAdapterProtocolFixture<TConfigureFixture, TConfigureClient, TConfigureServer> fixture)
        {
            Fixture = fixture;
            Client = fixture.Client;
            Server = fixture.Server;
            CancellationToken = fixture.CancellationToken;
            ClientEvents = fixture.ClientEvents;
            ServerEvents = fixture.ServerEvents;
            Events = fixture.Events;
            fixture.Swap(testOutputHelper);
        }

        protected IDebugAdapterServer Server { get; }
        protected IDebugAdapterClient Client { get; }
        protected CancellationToken CancellationToken { get; }
        public ISettler ClientEvents { get; }
        public ISettler ServerEvents { get; }
        public ISettler Events { get; }
        public Task SettleNext() => Events.SettleNext();
        public IObservable<Unit> Settle() => Events.Settle();
    }
}

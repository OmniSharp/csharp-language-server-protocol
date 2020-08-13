using System;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using OmniSharp.Extensions.DebugAdapter.Protocol.Client;
using OmniSharp.Extensions.DebugAdapter.Protocol.Server;
using OmniSharp.Extensions.DebugAdapter.Testing;
using OmniSharp.Extensions.JsonRpc.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Dap.Tests.Integration.Fixtures
{
    public class DebugAdapterProtocolFixture<TConfigureFixture, TConfigureClient, TConfigureServer> : DebugAdapterProtocolTestBase, IAsyncLifetime
        where TConfigureFixture : IConfigureDebugAdapterProtocolFixture, new()
        where TConfigureClient : IConfigureDebugAdapterClientOptions, new()
        where TConfigureServer : IConfigureDebugAdapterServerOptions, new()
    {
        private readonly TestLoggerFactory _loggerFactory;

        public DebugAdapterProtocolFixture() :
            base(new TConfigureFixture().Configure(new JsonRpcTestOptions(new TestLoggerFactory(null))))
        {
            _loggerFactory = TestOptions.ServerLoggerFactory as TestLoggerFactory;
        }

        public void Swap(ITestOutputHelper testOutputHelper)
        {
            _loggerFactory.Swap(testOutputHelper);
        }

        public IDebugAdapterClient Client { get; private set; }
        public IDebugAdapterServer Server { get; private set; }
        public new CancellationToken CancellationToken => base.CancellationToken;
        public new ISettler ClientEvents => base.ClientEvents;
        public new ISettler ServerEvents => base.ServerEvents;
        public new ISettler Events => base.Events;
        public new Task SettleNext() => Events.SettleNext();
        public new IObservable<Unit> Settle() => Events.Settle();

        public async Task InitializeAsync()
        {
            var (client, server) = await Initialize(new TConfigureClient().Configure, new TConfigureServer().Configure);
            Client = client;
            Server = server;
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
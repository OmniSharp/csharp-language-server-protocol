using System;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using OmniSharp.Extensions.DebugAdapter.Client;
using OmniSharp.Extensions.DebugAdapter.Server;
using OmniSharp.Extensions.DebugAdapter.Testing;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.JsonRpc.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Dap.Tests.Integration
{
    public class ConnectionAndDisconnectionTests : DebugAdapterProtocolTestBase
    {
        public ConnectionAndDisconnectionTests(ITestOutputHelper outputHelper) : base(
            new JsonRpcTestOptions().ConfigureForXUnit(outputHelper)
        )
        {
        }

        [Fact]
        public async Task Server_Should_Stay_Alive_When_Requests_Throw_An_Exception()
        {
            var (client, server) = await Initialize(ConfigureClient, ConfigureServer);

            var result = await client.SendRequest("keepalive").Returning<bool>(CancellationToken);
            result.Should().BeTrue();

            Func<Task> a = () => client.SendRequest("throw").ReturningVoid(CancellationToken);
            await a.Should().ThrowAsync<InternalErrorException>();

            result = await client.SendRequest("keepalive").Returning<bool>(CancellationToken);
            result.Should().BeTrue();
        }

        [Fact]
        public async Task Client_Should_Stay_Alive_When_Requests_Throw_An_Exception()
        {
            var (client, server) = await Initialize(ConfigureClient, ConfigureServer);

            var result = await server.SendRequest("keepalive").Returning<bool>(CancellationToken);
            result.Should().BeTrue();

            Func<Task> a = () => server.SendRequest("throw").ReturningVoid(CancellationToken);
            await a.Should().ThrowAsync<InternalErrorException>();

            result = await server.SendRequest("keepalive").Returning<bool>(CancellationToken);
            result.Should().BeTrue();
        }

        [Fact]
        public async Task Server_Should_Support_Links()
        {
            var (client, server) = await Initialize(ConfigureClient, ConfigureServer);

            var result = await client.SendRequest("ka").Returning<bool>(CancellationToken);
            result.Should().BeTrue();

            Func<Task> a = () => client.SendRequest("t").ReturningVoid(CancellationToken);
            await a.Should().ThrowAsync<InternalErrorException>();

            result = await client.SendRequest("ka").Returning<bool>(CancellationToken);
            result.Should().BeTrue();
        }

        [Fact]
        public async Task Client_Should_Support_Links()
        {
            var (client, server) = await Initialize(ConfigureClient, ConfigureServer);

            var result = await server.SendRequest("ka").Returning<bool>(CancellationToken);
            result.Should().BeTrue();

            Func<Task> a = () => server.SendRequest("t").ReturningVoid(CancellationToken);
            await a.Should().ThrowAsync<InternalErrorException>();

            result = await server.SendRequest("ka").Returning<bool>(CancellationToken);
            result.Should().BeTrue();
        }

        private void ConfigureClient(DebugAdapterClientOptions options)
        {
            options.OnRequest("keepalive", ct => Task.FromResult(true));
            options.WithLink("keepalive", "ka");
            options.WithLink("throw", "t");
            options.OnRequest(
                "throw", async ct => {
                    throw new NotSupportedException();
                    return Task.CompletedTask;
                }
            );
        }

        private void ConfigureServer(DebugAdapterServerOptions options)
        {
            options.OnRequest("keepalive", ct => Task.FromResult(true));
            options.WithLink("keepalive", "ka");
            options.WithLink("throw", "t");
            options.OnRequest(
                "throw", async ct => {
                    throw new NotSupportedException();
                    return Task.CompletedTask;
                }
            );
        }
    }
}

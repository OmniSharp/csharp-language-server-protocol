using FluentAssertions;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.JsonRpc.Testing;
using OmniSharp.Extensions.LanguageProtocol.Testing;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Server;
using Xunit.Abstractions;

#pragma warning disable CS0162

namespace Lsp.Integration.Tests
{
    public class ConnectionAndDisconnectionTests : LanguageProtocolTestBase
    {
        public ConnectionAndDisconnectionTests(ITestOutputHelper outputHelper) : base(
            new JsonRpcTestOptions().ConfigureForXUnit(outputHelper)
        )
        {
        }

        [Fact]
        public async Task Server_Should_Stay_Alive_When_Requests_Throw_An_Exception()
        {
            var (client, _) = await Initialize(ConfigureClient, ConfigureServer);

            var result = await client.SendRequest("keepalive").Returning<bool>(CancellationToken);
            result.Should().BeTrue();

            var a = () => client.SendRequest("throw").ReturningVoid(CancellationToken);
            await a.Should().ThrowAsync<InternalErrorException>();

            result = await client.SendRequest("keepalive").Returning<bool>(CancellationToken);
            result.Should().BeTrue();
        }

        [Fact]
        public async Task Client_Should_Stay_Alive_When_Requests_Throw_An_Exception()
        {
            var (_, server) = await Initialize(ConfigureClient, ConfigureServer);

            var result = await server.SendRequest("keepalive").Returning<bool>(CancellationToken);
            result.Should().BeTrue();

            var a = () => server.SendRequest("throw").ReturningVoid(CancellationToken);
            await a.Should().ThrowAsync<InternalErrorException>();

            result = await server.SendRequest("keepalive").Returning<bool>(CancellationToken);
            result.Should().BeTrue();
        }

        [Fact]
        public async Task Server_Should_Support_Links()
        {
            var (client, _) = await Initialize(ConfigureClient, ConfigureServer);

            var result = await client.SendRequest("ka").Returning<bool>(CancellationToken);
            result.Should().BeTrue();

            var a = () => client.SendRequest("t").ReturningVoid(CancellationToken);
            await a.Should().ThrowAsync<InternalErrorException>();

            result = await client.SendRequest("ka").Returning<bool>(CancellationToken);
            result.Should().BeTrue();
        }

        [Fact]
        public async Task Client_Should_Support_Links()
        {
            var (_, server) = await Initialize(ConfigureClient, ConfigureServer);

            var result = await server.SendRequest("ka").Returning<bool>(CancellationToken);
            result.Should().BeTrue();

            var a = () => server.SendRequest("t").ReturningVoid(CancellationToken);
            await a.Should().ThrowAsync<InternalErrorException>();

            result = await server.SendRequest("ka").Returning<bool>(CancellationToken);
            result.Should().BeTrue();
        }

        private void ConfigureClient(LanguageClientOptions options)
        {
            options.OnRequest("keepalive", ct => Task.FromResult(true));
            options.WithLink("keepalive", "ka");
            options.WithLink("throw", "t");
            options.OnRequest(
                "throw", async ct =>
                {
                    throw new NotSupportedException();
                    return Task.CompletedTask;
                }
            );
        }

        private void ConfigureServer(LanguageServerOptions options)
        {
            options.OnRequest("keepalive", ct => Task.FromResult(true));
            options.WithLink("keepalive", "ka");
            options.WithLink("throw", "t");
            options.OnRequest(
                "throw", async ct =>
                {
                    throw new NotSupportedException();
                    return Task.CompletedTask;
                }
            );
        }
    }
}

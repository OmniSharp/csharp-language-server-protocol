using System;
using System.IO.Pipelines;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.JsonRpc.Testing;
using OmniSharp.Extensions.LanguageProtocol.Testing;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Server;
using Xunit;
using Xunit.Abstractions;

namespace Lsp.Tests.Integration
{
    public class ConnectionAndDisconnectionTests : LanguageProtocolTestBase
    {
        public ConnectionAndDisconnectionTests(ITestOutputHelper outputHelper)  : base(new JsonRpcTestOptions()
            .ConfigureForXUnit(outputHelper)
            .WithTestTimeout(TimeSpan.FromSeconds(20))
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

        private void ConfigureClient(LanguageClientOptions options)
        {
            options.OnRequest("keepalive", (ct) => Task.FromResult(true));
            options.WithLink("keepalive", "ka");
            options.WithLink("throw", "t");
            options.OnRequest("throw", async ct => {
                throw new NotSupportedException();
                return Task.CompletedTask;
            });
        }

        private void ConfigureServer(LanguageServerOptions options)
        {
            options.OnRequest("keepalive", (ct) => Task.FromResult(true));
            options.WithLink("keepalive", "ka");
            options.WithLink("throw", "t");
            options.OnRequest("throw", async ct => {
                throw new NotSupportedException();
                return Task.CompletedTask;
            });
        }
    }
}

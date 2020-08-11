using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc.Testing;
using OmniSharp.Extensions.LanguageProtocol.Testing;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using OmniSharp.Extensions.LanguageServer.Server;
using Xunit;
using Xunit.Abstractions;

namespace Lsp.Tests.Integration
{
    public class InitializationTests : LanguageProtocolTestBase
    {
        public InitializationTests(ITestOutputHelper outputHelper) : base(new JsonRpcTestOptions().ConfigureForXUnit(outputHelper))
        {
        }

        [Fact]
        public async Task Logs_should_be_allowed_during_startup()
        {
            var (client, server) = await Initialize(ConfigureClient, ConfigureServer);

            _logs.Should().HaveCount(2);
            _logs.Should().ContainInOrder("OnInitialize", "OnInitialized");
        }

        private readonly List<string> _logs = new List<string>();

        private void ConfigureClient(LanguageClientOptions options) =>
            options.OnLogMessage(log => { _logs.Add(log.Message); });

        private void ConfigureServer(LanguageServerOptions options)
        {
            options.OnInitialize(
                (server, request, token) => {
                    server.Window.LogInfo("OnInitialize");
                    return Task.CompletedTask;
                }
            );
            options.OnInitialized(
                (server, request, response, token) => {
                    server.Window.LogInfo("OnInitialized");
                    return Task.CompletedTask;
                }
            );
        }
    }
}

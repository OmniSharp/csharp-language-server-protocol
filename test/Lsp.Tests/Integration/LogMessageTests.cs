using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc.Testing;
using OmniSharp.Extensions.LanguageProtocol.Testing;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using OmniSharp.Extensions.LanguageServer.Server;
using Xunit;
using Xunit.Abstractions;

namespace Lsp.Tests.Integration
{
    public class LogMessageTests : LanguageProtocolTestBase
    {
        public LogMessageTests(ITestOutputHelper outputHelper) : base(new JsonRpcTestOptions()
            .ConfigureForXUnit(outputHelper)
            .WithSettleTimeSpan(TimeSpan.FromMilliseconds(200))
        )
        {
        }

        private readonly List<LogMessageParams> _receivedMessages = new List<LogMessageParams>();

        [Fact]
        public async Task Should_Log_Messages_Through_Window_Extension_Methods()
        {
            var (client, server) = await Initialize(ConfigureClient, ConfigureServer);

            server.Window.LogError("Something bad happened...");
            server.Window.LogInfo("Here's something cool...");
            server.Window.LogWarning("Uh-oh...");
            server.Window.Log("Just gotta let you know!");
            server.Window.Log(new LogMessageParams() {
                Type = MessageType.Log, Message = "1234"
            });
            server.Window.LogMessage(new LogMessageParams() {
                Type = MessageType.Log, Message = "1234"
            });

            await Settle();

            _receivedMessages.Should().HaveCount(6);
            _receivedMessages.Should().Contain(z => z.Type == MessageType.Error);
            _receivedMessages.Should().Contain(z => z.Type == MessageType.Info);
            _receivedMessages.Should().Contain(z => z.Type == MessageType.Warning);
            _receivedMessages.Should().Contain(z => z.Type == MessageType.Log).And.Subject.Count(z => z.Type == MessageType.Log).Should().Be(3);
        }

        [Fact]
        public async Task Should_Log_Messages_Through_Server_Extension_Methods()
        {
            var (client, server) = await Initialize(ConfigureClient, ConfigureServer);

            server.LogError("Something bad happened...");
            server.LogInfo("Here's something cool...");
            server.LogWarning("Uh-oh...");
            server.Log("Just gotta let you know!");
            server.Log(new LogMessageParams() {
                Type = MessageType.Log, Message = "1234"
            });
            server.LogMessage(new LogMessageParams() {
                Type = MessageType.Log, Message = "1234"
            });

            await Settle();

            _receivedMessages.Should().HaveCount(6);
            _receivedMessages.Should().Contain(z => z.Type == MessageType.Error);
            _receivedMessages.Should().Contain(z => z.Type == MessageType.Info);
            _receivedMessages.Should().Contain(z => z.Type == MessageType.Warning);
            _receivedMessages.Should().Contain(z => z.Type == MessageType.Log).And.Subject.Count(z => z.Type == MessageType.Log).Should().Be(3);
        }

        private void ConfigureClient(LanguageClientOptions options)
        {
            options.OnLogMessage((request) => { _receivedMessages.Add(request); });
        }

        private void ConfigureServer(LanguageServerOptions options)
        {
            // options.OnCodeLens()
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ShowMessageTests : LanguageProtocolTestBase
    {
        public ShowMessageTests(ITestOutputHelper outputHelper) : base(new JsonRpcTestOptions().ConfigureForXUnit(outputHelper).WithSettleTimeSpan(TimeSpan.FromMilliseconds(500)))
        {
        }

        private readonly List<ShowMessageParams> _receivedMessages = new List<ShowMessageParams>();

        [Fact]
        public async Task Should_Show_Messages_Through_Window_Extension_Methods()
        {
            var (client, server) = await Initialize(ConfigureClient, ConfigureServer);

            server.Window.ShowError("Something bad happened...");
            server.Window.ShowInfo("Here's something cool...");
            server.Window.ShowWarning("Uh-oh...");
            server.Window.Show("Just gotta let you know!");
            server.Window.Show(
                new ShowMessageParams {
                    Type = MessageType.Log, Message = "1234"
                }
            );
            server.Window.ShowMessage(
                new ShowMessageParams {
                    Type = MessageType.Log, Message = "1234"
                }
            );

            await Task.Delay(1000);

            _receivedMessages.Should().HaveCount(6);
            _receivedMessages.Should().Contain(z => z.Type == MessageType.Error);
            _receivedMessages.Should().Contain(z => z.Type == MessageType.Info);
            _receivedMessages.Should().Contain(z => z.Type == MessageType.Warning);
            _receivedMessages.Should().Contain(z => z.Type == MessageType.Log).And.Subject.Count(z => z.Type == MessageType.Log).Should().Be(3);
        }

        [Fact]
        public async Task Should_Show_Messages_Through_Server_Extension_Methods()
        {
            var (client, server) = await Initialize(ConfigureClient, ConfigureServer);

            server.ShowError("Something bad happened...");
            server.ShowInfo("Here's something cool...");
            server.ShowWarning("Uh-oh...");
            server.Show("Just gotta let you know!");
            server.Show(
                new ShowMessageParams {
                    Type = MessageType.Log, Message = "1234"
                }
            );
            server.ShowMessage(
                new ShowMessageParams {
                    Type = MessageType.Log, Message = "1234"
                }
            );

            await Task.Delay(1000);

            _receivedMessages.Should().HaveCount(6);
            _receivedMessages.Should().Contain(z => z.Type == MessageType.Error);
            _receivedMessages.Should().Contain(z => z.Type == MessageType.Info);
            _receivedMessages.Should().Contain(z => z.Type == MessageType.Warning);
            _receivedMessages.Should().Contain(z => z.Type == MessageType.Log).And.Subject.Count(z => z.Type == MessageType.Log).Should().Be(3);
        }

        private void ConfigureClient(LanguageClientOptions options) => options.OnShowMessage(request => { _receivedMessages.Add(request); });

        private void ConfigureServer(LanguageServerOptions options)
        {
            // options.OnCodeLens()
        }
    }
}

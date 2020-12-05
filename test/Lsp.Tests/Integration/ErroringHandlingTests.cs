using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.JsonRpc.Testing;
using OmniSharp.Extensions.LanguageProtocol.Testing;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using TestingUtils;
using Xunit;
using Xunit.Abstractions;

namespace Lsp.Tests.Integration
{
    public class ErroringHandlingTests : LanguageProtocolTestBase
    {
        public ErroringHandlingTests(ITestOutputHelper outputHelper) : base(new JsonRpcTestOptions().ConfigureForXUnit(outputHelper))
        {
        }

        [RetryFact]
        public async Task Should_Handle_Malformed_Request()
        {
            var (client, server) = await Initialize(ConfigureClient, ConfigureServer);

            var codeActionParams = new {
                Range = new {
                    Start = new { Line = 1, Character = double.Parse("1.0E300") },
                    End = new { Line = 2, Character = 9999999999999999999 }
                },
                TextDocument = new {
                    Uri = DocumentUri.From("/path/to/file")
                },
                Context = new {
                    Diagnostics = new Container<Diagnostic>()
                }
            };

            var req = client.SendRequest(TextDocumentNames.CodeAction, codeActionParams);
            Func<Task> a = () => req.Returning<object>(CancellationToken);
            a.Should().Throw<ParseErrorException>();
        }

        [RetryFact]
        public async Task Should_Handle_Malformed_Notification()
        {
            var (client, server) = await Initialize(ConfigureClient, ConfigureServer);

            var notification = new {
                ContentChanges = new[] {
                    new {
                        Text = "Text change",
                        Range = new {
                            Start = new { Line = 1, Character = double.Parse("1.0E300") },
                            End = new { Line = 2, Character = 9999999999999999999 }
                        },
                    }
                },
                TextDocument = new {
                    Uri = DocumentUri.From("/path/to/file")
                },
            };

            Action a = () => client.SendNotification(TextDocumentNames.DidChange, notification);
            a.Should().NotThrow();
        }


        private void ConfigureClient(LanguageClientOptions options)
        {
        }

        private void ConfigureServer(LanguageServerOptions options)
        {
            options.OnCodeAction(@params => Task.FromResult(new CommandOrCodeActionContainer()), (capability, capabilities) => new CodeActionRegistrationOptions());
        }
    }
}

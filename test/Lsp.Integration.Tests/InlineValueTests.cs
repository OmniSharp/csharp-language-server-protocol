using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc.Testing;
using OmniSharp.Extensions.LanguageProtocol.Testing;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Server;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

namespace Lsp.Integration.Tests
{
    public class InlineValueTests : LanguageProtocolTestBase
    {
        private readonly Func<InlineValueParams, CancellationToken, Task<Container<InlineValueBase>?>> _request;

        public InlineValueTests(ITestOutputHelper outputHelper) : base(new JsonRpcTestOptions().ConfigureForXUnit(outputHelper, LogEventLevel.Verbose))
        {
            _request = Substitute.For<Func<InlineValueParams, CancellationToken, Task<Container<InlineValueBase>?>>>();
        }

        [Fact]
        public async Task Should_Get_InlineValues()
        {
            _request.Invoke(Arg.Any<InlineValueParams>(), Arg.Any<CancellationToken>())
                    .Returns(
                         new Container<InlineValueBase>(
                             new InlineValueText()
                             {
                                 Range = new (new (1, 1), (1, 2)),
                                 Text = "Text Value",
                             },
                             new InlineValueEvaluatableExpression()
                             {
                                 Range = new (new (3, 1), (3, 2)),
                                 Expression = "<expression>"
                             },
                             new InlineValueVariableLookup()
                             {
                                 Range = new (new (2, 1), (2, 2)),
                                 VariableName = "value",
                                 CaseSensitiveLookup = true
                             }
                         )
                     );


            var (client, _) = await Initialize(ClientOptionsAction, ServerOptionsAction);

            var result = await client.RequestInlineValues(new InlineValueParams(), CancellationToken);

            result.Should().HaveCount(3);
            result.Select(z => z.GetType()).Should().ContainInOrder(
                typeof(InlineValueText), typeof(InlineValueEvaluatableExpression), typeof(InlineValueVariableLookup)
            );
        }

        private void ServerOptionsAction(LanguageServerOptions obj)
        {
            obj.OnInlineValues(
                _request, (_, _) => new InlineValueRegistrationOptions
                {
                    DocumentSelector = TextDocumentSelector.ForLanguage("csharp"),
                }
            );
        }

        private void ClientOptionsAction(LanguageClientOptions obj)
        {
        }
    }
}

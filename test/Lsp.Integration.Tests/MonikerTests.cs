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
    public class MonikerTests : LanguageProtocolTestBase
    {
        private readonly Func<MonikerParams, CancellationToken, Task<Container<Moniker>?>> _request;

        public MonikerTests(ITestOutputHelper outputHelper) : base(new JsonRpcTestOptions().ConfigureForXUnit(outputHelper, LogEventLevel.Verbose))
        {
            _request = Substitute.For<Func<MonikerParams, CancellationToken, Task<Container<Moniker>?>>>();
        }

        [Fact]
        public async Task Should_Get_Monikers()
        {
            _request.Invoke(Arg.Any<MonikerParams>(), Arg.Any<CancellationToken>())
                    .Returns(
                         new Container<Moniker>(
                             new Moniker
                             {
                                 Identifier = "abcd",
                                 Kind = MonikerKind.Export,
                                 Scheme = "http",
                                 Unique = UniquenessLevel.Document
                             }
                         )
                     );


            var (client, _) = await Initialize(ClientOptionsAction, ServerOptionsAction);

            var result = await client.RequestMoniker(new MonikerParams(), CancellationToken);

            result.Should().HaveCount(1);
            result.Should().Match(z => z.Any(x => x.Kind == MonikerKind.Export));
        }

        private void ServerOptionsAction(LanguageServerOptions obj)
        {
            obj.OnMoniker(
                _request, (_, _) => new MonikerRegistrationOptions
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

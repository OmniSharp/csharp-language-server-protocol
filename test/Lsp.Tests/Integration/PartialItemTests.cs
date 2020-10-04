using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using FluentAssertions;
using Lsp.Tests.Integration.Fixtures;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc.Testing;
using OmniSharp.Extensions.LanguageProtocol.Testing;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Document.Proposals;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Server;
using TestingUtils;
using Xunit;
using Xunit.Abstractions;

namespace Lsp.Tests.Integration
{
    public static class PartialItemTests
    {
        public class Delegates : LanguageProtocolFixtureTest<DefaultOptions, DefaultClient, Delegates.DelegateServer>
        {
            public Delegates(ITestOutputHelper testOutputHelper, LanguageProtocolFixture<DefaultOptions, DefaultClient, DelegateServer> fixture) : base(testOutputHelper, fixture)
            {
            }

            [FactWithSkipOn(SkipOnPlatform.All)]
            public async Task Should_Behave_Like_A_Task()
            {
                var result = await Client.TextDocument.RequestSemanticTokens(
                    new SemanticTokensParams() { TextDocument = new TextDocumentIdentifier(@"c:\test.cs") }, CancellationToken
                );
                result.Should().NotBeNull();
                result.Data.Should().HaveCount(3);
            }

            [FactWithSkipOn(SkipOnPlatform.All)]
            public async Task Should_Behave_Like_An_Observable()
            {
                var items = await Client.TextDocument
                                        .RequestSemanticTokens(
                                             new SemanticTokensParams() { TextDocument = new TextDocumentIdentifier(@"c:\test.cs") }, CancellationToken
                                         )
                                        .Aggregate(
                                             new List<SemanticTokensPartialResult>(), (acc, v) => {
                                                 acc.Add(v);
                                                 return acc;
                                             }
                                         )
                                        .ToTask(CancellationToken);

                items.Should().HaveCount(3);
                items.Select(z => z.Data.Length).Should().ContainInOrder(1, 2, 3);
            }

            [Fact]
            public async Task Should_Behave_Like_An_Observable_Without_Progress_Support()
            {
                var response = await Client.SendRequest(new SemanticTokensParams { TextDocument = new TextDocumentIdentifier(@"c:\test.cs") }, CancellationToken);

                response.Should().NotBeNull();
                response.Data.Should().HaveCount(3);
            }

            public class DelegateServer : IConfigureLanguageServerOptions
            {
                public void Configure(LanguageServerOptions options) => options.OnSemanticTokens(
                    (@params, observer, arg3) => {
                        observer.OnNext(new SemanticTokensPartialResult() { Data = new[] { 0 }.ToImmutableArray() });
                        observer.OnNext(new SemanticTokensPartialResult() { Data = new[] { 0, 1 }.ToImmutableArray() });
                        observer.OnNext(new SemanticTokensPartialResult() { Data = new[] { 0, 1, 2 }.ToImmutableArray() });
                        observer.OnCompleted();
                    }, new SemanticTokensRegistrationOptions() {
                    }
                );
            }
        }
    }
}

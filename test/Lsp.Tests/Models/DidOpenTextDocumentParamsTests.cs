using System;
using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;

namespace Lsp.Tests.Models
{
    public class DidOpenTextDocumentParamsTests
    {
        [Theory]
        [JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new DidOpenTextDocumentParams {
                TextDocument = new TextDocumentItem("csharp", new Uri("file:///abc/def.cs")) {
                    Version = 1,
                    Text = "content"
                }
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<DidOpenTextDocumentParams>(expected);
            deresult.Should().BeEquivalentTo(model);
        }
    }
}

using System;
using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;

namespace Lsp.Tests.Models
{
    public class DidSaveTextDocumentParamsTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new DidSaveTextDocumentParams() {
                Text = "content",
                TextDocument = new TextDocumentIdentifier(new Uri("file:///abc/def.cs"))
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<DidSaveTextDocumentParams>(expected);
            deresult.Should().BeEquivalentTo(model);
        }
    }
}

using System;
using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;

namespace Lsp.Tests.Models
{
    public class DocumentLinkParamsTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new DocumentLinkParams<ResolvedData>() {
                TextDocument = new TextDocumentIdentifier(new Uri("file:///abc/123.cs"))
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<DocumentLinkParams<ResolvedData>>(expected);
            deresult.Should().BeEquivalentTo(model);
        }
    }
}

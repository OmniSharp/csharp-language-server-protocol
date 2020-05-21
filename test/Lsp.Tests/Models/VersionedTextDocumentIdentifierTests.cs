using System;
using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;

namespace Lsp.Tests.Models
{
    public class VersionedTextDocumentIdentifierTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new VersionedTextDocumentIdentifier()
            {
                Uri = new Uri("file:///abc/123.cs"),
                Version = 12
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<VersionedTextDocumentIdentifier>(expected);
            deresult.Should().BeEquivalentTo(model);
        }
    }
}

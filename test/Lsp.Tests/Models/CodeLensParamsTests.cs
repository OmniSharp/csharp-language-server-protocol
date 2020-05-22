using System;
using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;

namespace Lsp.Tests.Models
{
    public class CodeLensParamsTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new CodeLensParams() {
                TextDocument = new TextDocumentIdentifier(new Uri("file:///abc/123/d.cs")),
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<CodeLensParams>(expected);
            deresult.Should().BeEquivalentTo(model);
        }

        [Theory, JsonFixture]
        public void NonStandardCharactersTest(string expected)
        {
            var model = new CodeLensParams() {
                // UNC path with Chinese character for tree.
                TextDocument = new TextDocumentIdentifier(DocumentUri.FromFileSystemPath("\\\\abc\\123\\树.cs")),
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<CodeLensParams>(expected);
            deresult.Should().BeEquivalentTo(model);
        }
    }
}

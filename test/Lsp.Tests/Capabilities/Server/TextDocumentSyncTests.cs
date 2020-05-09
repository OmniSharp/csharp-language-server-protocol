using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using Xunit;

namespace Lsp.Tests.Capabilities.Server
{
    public class TextDocumentSyncTests
    {
        [Theory, JsonFixture]
        public void TextDocumentSyncKind_Full(string expected)
        {
            var model = new TextDocumentSync(TextDocumentSyncKind.Full);
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<TextDocumentSync>(expected);
            deresult.Should().BeEquivalentTo(model);
        }

        [Theory, JsonFixture]
        public void TextDocumentSyncKind_Incremental(string expected)
        {
            var model = new TextDocumentSync(TextDocumentSyncKind.Incremental);
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<TextDocumentSync>(expected);
            deresult.Should().BeEquivalentTo(model);
        }

        [Theory, JsonFixture]
        public void TextDocumentSyncKind_None(string expected)
        {
            var model = new TextDocumentSync(TextDocumentSyncKind.None);
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<TextDocumentSync>(expected);
            deresult.Should().BeEquivalentTo(model);
        }

        [Theory, JsonFixture]
        public void TextDocumentSyncOptions(string expected)
        {
            var model = new TextDocumentSync(new TextDocumentSyncOptions()
            {
                OpenClose = true,
                Change = TextDocumentSyncKind.Full,
                Save = new SaveOptions()
                {
                    IncludeText = true
                },
                WillSave = true,
                WillSaveWaitUntil = true
            });
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<TextDocumentSync>(expected);
            deresult.Should().BeEquivalentTo(model);
        }
    }
}

using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using Xunit;

namespace Lsp.Tests.Capabilities.Server
{
    public class TextDocumentSyncOptionsTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new TextDocumentSyncOptions() {
                Change = TextDocumentSyncKind.Full,
                OpenClose = true,
                Save = new SaveOptions() {
                    IncludeText = true
                },
                WillSave = true,
                WillSaveWaitUntil = true
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<TextDocumentSyncOptions>(expected);
            deresult.Should().BeEquivalentTo(model);
        }
    }
}

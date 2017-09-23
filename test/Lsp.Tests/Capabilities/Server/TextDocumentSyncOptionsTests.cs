using System;
using FluentAssertions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServerProtocol.Capabilities.Server;
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

            var deresult = JsonConvert.DeserializeObject<TextDocumentSyncOptions>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}

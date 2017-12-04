using System;
using FluentAssertions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Xunit;

namespace Lsp.Tests.Models
{
    public class DidOpenTextDocumentParamsTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new DidOpenTextDocumentParams() {
                TextDocument = new TextDocumentItem() {
                    Uri = new Uri("file:///abc/def.cs"),
                    LanguageId = "csharp",
                    Text = "content",
                    Version = 1
                }
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<DidOpenTextDocumentParams>(expected, Serializer.CreateSerializerSettings(ClientVersion.Lsp3));
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}

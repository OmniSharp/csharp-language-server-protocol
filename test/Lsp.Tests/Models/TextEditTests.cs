using System;
using FluentAssertions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Xunit;

namespace Lsp.Tests.Models
{
    public class TextEditTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new TextEdit() {
                NewText = "new text",
                Range = new Range(new Position(1, 1), new Position(2, 2))
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<TextEdit>(expected, Serializer.CreateSerializerSettings(ClientVersion.Lsp3));
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}

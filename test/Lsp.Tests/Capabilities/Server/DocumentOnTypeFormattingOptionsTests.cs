using System;
using System.Linq;
using FluentAssertions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServerProtocol.Capabilities.Server;
using Xunit;

namespace Lsp.Tests.Capabilities.Server
{
    public class DocumentOnTypeFormattingOptionsTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new DocumentOnTypeFormattingOptions() {
                FirstTriggerCharacter = ".",
                MoreTriggerCharacter = ";`".Select(x => x.ToString()).ToArray(),
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<DocumentOnTypeFormattingOptions>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }

        [Theory, JsonFixture]
        public void Optional(string expected)
        {
            var model = new DocumentOnTypeFormattingOptions() {
                FirstTriggerCharacter = "."
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<DocumentOnTypeFormattingOptions>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}

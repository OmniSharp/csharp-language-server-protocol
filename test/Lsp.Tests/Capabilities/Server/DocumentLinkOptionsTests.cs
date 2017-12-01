using System;
using FluentAssertions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using Xunit;

namespace Lsp.Tests.Capabilities.Server
{
    public class DocumentLinkOptionsTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new DocumentLinkOptions() {
                ResolveProvider = true,
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<DocumentLinkOptions>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}

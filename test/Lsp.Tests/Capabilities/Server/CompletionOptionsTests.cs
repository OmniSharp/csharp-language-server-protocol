using System;
using FluentAssertions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServerProtocol.Capabilities.Server;
using Xunit;

namespace Lsp.Tests.Capabilities.Server
{
    public class CompletionOptionsTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new CompletionOptions() {
                ResolveProvider = false,
            };
            var result = Fixture.SerializeObject(model);
            
            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<CompletionOptions>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}

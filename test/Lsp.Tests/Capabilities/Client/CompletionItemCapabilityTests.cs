using System;
using FluentAssertions;
using Lsp.Capabilities.Client;
using Newtonsoft.Json;
using Xunit;

namespace Lsp.Tests.Capabilities.Client
{
    public class CompletionItemCapabilityTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new CompletionItemCapability() {
                SnippetSupport = true
            };
            var result = Fixture.SerializeObject(model);
            
            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<CompletionItemCapability>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}

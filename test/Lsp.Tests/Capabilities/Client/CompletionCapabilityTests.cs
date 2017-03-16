using System;
using FluentAssertions;
using Lsp.Capabilities.Client;
using Newtonsoft.Json;
using Xunit;

namespace Lsp.Tests.Capabilities.Client
{
    public class CompletionCapabilityTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new CompletionCapability();
            var result = Fixture.SerializeObject(model);
            
            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<CompletionCapability>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}

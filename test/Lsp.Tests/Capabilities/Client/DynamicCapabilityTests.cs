using System;
using FluentAssertions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Capabilities.Client;
using Xunit;

namespace Lsp.Tests.Capabilities.Client
{
    public class DynamicCapabilityTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new DynamicCapability();
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<DynamicCapability>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}

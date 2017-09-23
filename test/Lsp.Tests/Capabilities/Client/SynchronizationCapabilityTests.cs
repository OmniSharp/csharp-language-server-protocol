using System;
using FluentAssertions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Capabilities.Client;
using Xunit;

namespace Lsp.Tests.Capabilities.Client
{
    public class SynchronizationCapabilityTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new SynchronizationCapability();
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<SynchronizationCapability>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}

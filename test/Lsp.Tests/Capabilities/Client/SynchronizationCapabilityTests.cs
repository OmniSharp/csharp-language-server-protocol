using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;

namespace Lsp.Tests.Capabilities.Client
{
    public class SynchronizationCapabilityTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new SynchronizationCapability()
            {
                WillSave = false,
                WillSaveWaitUntil = false,
                DidSave = false,
                DynamicRegistration = false
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<SynchronizationCapability>(expected);
            deresult.Should().BeEquivalentTo(model);
        }
    }
}

using System.Text.Json;
using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;

namespace Lsp.Tests.Capabilities.Client
{
    public class PublishDiagnosticsCapabilityTests
    {
        [Fact]
        public void TagSupportTrue()
        {
            var deresult = JsonSerializer.Deserialize<PublishDiagnosticsCapability>("{\"tagSupport\":true}", Serializer.Instance.Options);
            deresult.Should().BeEquivalentTo(new PublishDiagnosticsCapability() {
                TagSupport = new Supports<PublishDiagnosticsTagSupportCapability>(true)
            });
        }

        [Fact]
        public void TagSupportObject()
        {
            var deresult = JsonSerializer.Deserialize<PublishDiagnosticsCapability>("{\"tagSupport\":{\"valueSet\": [2,1]}}", Serializer.Instance.Options);
            deresult.TagSupport.IsSupported.Should().Be(true);
            deresult.TagSupport.Value.ValueSet.Should().ContainInOrder(DiagnosticTag.Deprecated, DiagnosticTag.Unnecessary);
        }
    }
}

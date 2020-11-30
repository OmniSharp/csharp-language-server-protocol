using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using TestingUtils;
using Xunit;

namespace Lsp.Tests.Capabilities.Client
{
    public class PublishDiagnosticsCapabilityTests
    {
        [Fact]
        public void TagSupportTrue()
        {
            var deresult = new LspSerializer(ClientVersion.Lsp3).DeserializeObject<PublishDiagnosticsCapability>("{\"tagSupport\":true}");
            deresult.Should().BeEquivalentTo(
                new PublishDiagnosticsCapability {
                    TagSupport = new Supports<PublishDiagnosticsTagSupportCapabilityOptions?>(true)
                }, x => x.UsingStructuralRecordEquality()
            );
        }

        [Fact]
        public void TagSupportObject()
        {
            var deresult = new LspSerializer(ClientVersion.Lsp3).DeserializeObject<PublishDiagnosticsCapability>("{\"tagSupport\":{\"valueSet\": [2,1]}}");
            deresult.TagSupport.IsSupported.Should().Be(true);
            deresult.TagSupport.Value!.ValueSet.Should().ContainInOrder(DiagnosticTag.Deprecated, DiagnosticTag.Unnecessary);
        }
    }
}

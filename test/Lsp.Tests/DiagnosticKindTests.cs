using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;

namespace Lsp.Tests
{
    public class DiagnosticKindTests
    {
        [Fact]
        public void DefaultBehavior_Should_Only_Support_InitialDiagnosticTags()
        {
            var serializer = new LspSerializer();
            var json = serializer.SerializeObject(
                new Diagnostic {
                    Tags = new Container<DiagnosticTag>(DiagnosticTag.Deprecated)
                }
            );

            var result = serializer.DeserializeObject<Diagnostic>(json);
            result.Tags.Should().Contain(DiagnosticTag.Deprecated);
        }
    }
}

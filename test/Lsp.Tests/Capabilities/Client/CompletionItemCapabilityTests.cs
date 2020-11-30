using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using TestingUtils;
using Xunit;

namespace Lsp.Tests.Capabilities.Client
{
    public class CompletionItemCapabilityTests
    {
        [Theory]
        [JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new CompletionItemCapabilityOptions {
                SnippetSupport = true
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new LspSerializer(ClientVersion.Lsp3).DeserializeObject<CompletionItemCapabilityOptions>(expected);
            deresult.Should().BeEquivalentTo(model, x => x.UsingStructuralRecordEquality());
        }

        [Fact]
        public void TagSupportTrue()
        {
            var deresult = new LspSerializer(ClientVersion.Lsp3).DeserializeObject<CompletionItemCapabilityOptions>("{\"tagSupport\":true}");
            deresult.Should().BeEquivalentTo(
                new CompletionItemCapabilityOptions {
                    TagSupport = new Supports<CompletionItemTagSupportCapabilityOptions?>(true)
                }, x => x.UsingStructuralRecordEquality()
            );
        }

        [Fact]
        public void TagSupportObject()
        {
            var deresult = new LspSerializer(ClientVersion.Lsp3).DeserializeObject<CompletionItemCapabilityOptions>("{\"tagSupport\":{\"valueSet\": [1]}}");
            deresult.TagSupport.IsSupported.Should().Be(true);
            deresult.TagSupport.Value!.ValueSet.Should().Contain(CompletionItemTag.Deprecated);
        }
    }
}

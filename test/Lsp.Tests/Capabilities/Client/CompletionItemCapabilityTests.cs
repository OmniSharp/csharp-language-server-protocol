using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;

namespace Lsp.Tests.Capabilities.Client
{
    public class CompletionItemCapabilityTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new CompletionItemCapability()
            {
                SnippetSupport = true
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<CompletionItemCapability>(expected);
            deresult.Should().BeEquivalentTo(model);
        }

        [Fact]
        public void TagSupportTrue()
        {
            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<CompletionItemCapability>("{\"tagSupport\":true}");
            deresult.Should().BeEquivalentTo(new CompletionItemCapability() {
                TagSupport = new Supports<CompletionItemTagSupportCapability>(true)
            });
        }

        [Fact]
        public void TagSupportObject()
        {
            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<CompletionItemCapability>("{\"tagSupport\":{\"valueSet\": [1]}}");
            deresult.TagSupport.IsSupported.Should().Be(true);
            deresult.TagSupport.Value.ValueSet.Should().Contain(CompletionItemTag.Deprecated);
        }
    }
}

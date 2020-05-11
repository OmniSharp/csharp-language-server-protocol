using System.Text.Json;
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

            var deresult = JsonSerializer.Deserialize<CompletionItemCapability>(expected, Serializer.Instance.Options);
            deresult.Should().BeEquivalentTo(model);
        }

        [Fact]
        public void TagSupportTrue()
        {
            var deresult = JsonSerializer.Deserialize<CompletionItemCapability>("{\"tagSupport\":true}", Serializer.Instance.Options);
            deresult.Should().BeEquivalentTo(new CompletionItemCapability() {
                TagSupport = new Supports<CompletionItemTagSupportCapability>(true)
            });
        }

        [Fact]
        public void TagSupportObject()
        {
            var deresult = JsonSerializer.Deserialize<CompletionItemCapability>("{\"tagSupport\":{\"valueSet\": [1]}}", Serializer.Instance.Options);
            deresult.TagSupport.IsSupported.Should().Be(true);
            deresult.TagSupport.Value.ValueSet.Should().Contain(CompletionItemTag.Deprecated);
        }
    }
}

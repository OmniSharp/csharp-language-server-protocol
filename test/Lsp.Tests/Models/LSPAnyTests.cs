using System;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;

namespace Lsp.Tests.Models
{
    public class LSPAnyTests
    {
        [Fact]
        public void Deserializes_And_Serializes_Arbitrary_Lsp_Values()
        {
            const string expected = """
            {
                "value": {
                    "name": "example",
                    "items": [
                        1,
                        true,
                        null
                    ]
                }
            }
            """;

            var serializer = new LspSerializer(ClientVersion.Lsp3);
            var result = serializer.DeserializeObject<LSPAnyContainer>(expected);

            result.Value.Value.Should().BeOfType<JObject>();
            Fixture.SerializeObject(result).Should().Be(expected.Replace("\r\n", "\n", StringComparison.Ordinal));
        }

        [Fact]
        public void Provides_Lsp_Object_And_Array_Model_Types()
        {
            var value = LSPAny.From(
                new LSPObject
                {
                    ["items"] = new LSPArray(1, "two", false)
                }
            );

            value.Value.Should().BeOfType<LSPObject>();
            value.Value!["items"].Should().BeOfType<LSPArray>();
            value.ToString().Should().Be("""{"items":[1,"two",false]}""");
        }

        private class LSPAnyContainer
        {
            public LSPAny Value { get; init; }
        }
    }
}

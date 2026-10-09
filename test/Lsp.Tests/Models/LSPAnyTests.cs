using System.Text.Json;
using FluentAssertions;
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

            result.Value.Value.ValueKind.Should().Be(JsonValueKind.Object);
            var serialized = Fixture.SerializeObject(result);
            using var expectedDocument = JsonDocument.Parse(expected);
            using var serializedDocument = JsonDocument.Parse(serialized);
            JsonElement.DeepEquals(expectedDocument.RootElement, serializedDocument.RootElement).Should().BeTrue();
        }

        [Fact]
        public void Provides_Lsp_Object_And_Array_Model_Types()
        {
            var value = LSPAny.From(
                new LSPObject
                {
                    ["nested"] = new LSPObject { ["items"] = new LSPArray(1, "two", false) }
                }
            );

            value.Value.ValueKind.Should().Be(JsonValueKind.Object);
            value.Value.GetProperty("nested").GetProperty("items").ValueKind.Should().Be(JsonValueKind.Array);
            value.ToString().Should().Be("""{"nested":{"items":[1,"two",false]}}""");
        }

        [Fact]
        public void Supports_Null_And_Structural_Equality()
        {
            var left = LSPAny.From(new { name = "example", enabled = true });
            using var document = JsonDocument.Parse("""{"name":"example","enabled":true}""");
            var right = LSPAny.From(document.RootElement);

            left.Should().Be(right);
            LSPAny.From(null).Value.ValueKind.Should().Be(JsonValueKind.Null);
        }

        private class LSPAnyContainer
        {
            public LSPAny Value { get; init; }
        }
    }
}
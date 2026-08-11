using System.Text.Json;
using System.Text.Json.Nodes;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// The LSP any type.
    ///
    /// @since 3.17.0
    /// </summary>
    [Newtonsoft.Json.JsonConverter(typeof(Converter))]
    public readonly partial struct LSPAny : IEquatable<LSPAny>
    {
        public LSPAny(JsonElement value)
        {
            Value = value.Clone();
        }

        public JsonElement Value { get; }

        public static LSPAny From(object? value) =>
            value switch
            {
                LSPAny any           => any,
                JsonElement element  => new LSPAny(element),
                LSPObject @object    => new LSPAny(JsonSerializer.SerializeToElement(@object.Value)),
                LSPArray array       => new LSPAny(JsonSerializer.SerializeToElement(array.Value)),
                _                    => new LSPAny(JsonSerializer.SerializeToElement(value))
            };

        public bool Equals(LSPAny other) => JsonElement.DeepEquals(Value, other.Value);

        public override bool Equals(object? obj) => obj is LSPAny other && Equals(other);

        public override int GetHashCode() => 0;

        public override string ToString() => Value.ValueKind == JsonValueKind.Undefined ? "null" : Value.GetRawText();

        public static implicit operator LSPAny(JsonElement value) => new(value);

        public static implicit operator JsonElement(LSPAny value) => value.Value;

        public static bool operator ==(LSPAny left, LSPAny right) => left.Equals(right);

        public static bool operator !=(LSPAny left, LSPAny right) => !left.Equals(right);

        internal static JsonNode? ToJsonNode(object? value) =>
            value switch
            {
                null                => null,
                JsonNode node       => node.DeepClone(),
                JsonElement element => JsonNode.Parse(element.GetRawText()),
                LSPAny any          => JsonNode.Parse(any.ToString()),
                LSPObject @object   => @object.Value.DeepClone(),
                LSPArray array      => array.Value.DeepClone(),
                _                   => JsonSerializer.SerializeToNode(value)
            };

        internal class Converter : Newtonsoft.Json.JsonConverter<LSPAny>
        {
            public override void WriteJson(
                Newtonsoft.Json.JsonWriter writer, LSPAny value, Newtonsoft.Json.JsonSerializer serializer
            )
            {
                writer.WriteRawValue(value.ToString());
            }

            public override LSPAny ReadJson(
                Newtonsoft.Json.JsonReader reader,
                Type objectType,
                LSPAny existingValue,
                bool hasExistingValue,
                Newtonsoft.Json.JsonSerializer serializer
            )
            {
                return new LSPAny(serializer.Deserialize<JsonElement>(reader));
            }
        }
    }

    /// <summary>
    /// LSP object definition.
    ///
    /// @since 3.17.0
    /// </summary>
    public partial class LSPObject
    {
        public LSPObject()
        {
        }

        public LSPObject(params object[] content)
        {
            foreach (var item in content)
            {
                if (LSPAny.ToJsonNode(item) is not JsonObject value)
                    throw new ArgumentException("LSP object content must serialize to a JSON object.", nameof(content));

                foreach (var property in value)
                {
                    Value[property.Key] = property.Value?.DeepClone();
                }
            }
        }

        public JsonNode? this[string propertyName]
        {
            get => Value[propertyName];
            set => Value[propertyName] = value;
        }

        public JsonObject Value { get; } = new();

        public override string ToString() => Value.ToJsonString();

        public static implicit operator JsonNode(LSPObject value) => value.Value.DeepClone();
    }

    /// <summary>
    /// LSP arrays.
    ///
    /// @since 3.17.0
    /// </summary>
    public partial class LSPArray
    {
        public LSPArray()
        {
        }

        public LSPArray(params object[] content)
        {
            foreach (var item in content)
            {
                Value.Add(LSPAny.ToJsonNode(item));
            }
        }

        public JsonArray Value { get; } = new();

        public override string ToString() => Value.ToJsonString();

        public static implicit operator JsonNode(LSPArray value) => value.Value.DeepClone();
    }
}
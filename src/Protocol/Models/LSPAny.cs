using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// The LSP any type.
    ///
    /// @since 3.17.0
    /// </summary>
    [JsonConverter(typeof(Converter))]
    public readonly partial struct LSPAny : IEquatable<LSPAny>
    {
        public LSPAny(JToken? value)
        {
            Value = value;
        }

        public JToken? Value { get; }

        public static LSPAny From(object? value) =>
            value switch
            {
                null         => new LSPAny(JValue.CreateNull()),
                LSPAny any   => any,
                JToken token => new LSPAny(token),
                _            => new LSPAny(JToken.FromObject(value))
            };

        public bool Equals(LSPAny other) => JToken.DeepEquals(Value, other.Value);

        public override bool Equals(object? obj) => obj is LSPAny other && Equals(other);

        public override int GetHashCode() => Value?.GetHashCode() ?? 0;

        public override string ToString() => Value?.ToString(Formatting.None) ?? "null";

        public static implicit operator LSPAny(JToken? value) => new(value);

        public static implicit operator JToken?(LSPAny value) => value.Value;

        public static bool operator ==(LSPAny left, LSPAny right) => left.Equals(right);

        public static bool operator !=(LSPAny left, LSPAny right) => !left.Equals(right);

        internal class Converter : JsonConverter<LSPAny>
        {
            public override void WriteJson(JsonWriter writer, LSPAny value, JsonSerializer serializer)
            {
                if (value.Value is null)
                {
                    writer.WriteNull();
                    return;
                }

                value.Value.WriteTo(writer);
            }

            public override LSPAny ReadJson(JsonReader reader, Type objectType, LSPAny existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                return reader.TokenType == JsonToken.Null
                    ? new LSPAny(JValue.CreateNull())
                    : new LSPAny(JToken.ReadFrom(reader));
            }
        }
    }

    /// <summary>
    /// LSP object definition.
    ///
    /// @since 3.17.0
    /// </summary>
    public partial class LSPObject : JObject
    {
        public LSPObject()
        {
        }

        public LSPObject(params object[] content) : base(content)
        {
        }
    }

    /// <summary>
    /// LSP arrays.
    ///
    /// @since 3.17.0
    /// </summary>
    public partial class LSPArray : JArray
    {
        public LSPArray()
        {
        }

        public LSPArray(params object[] content) : base(content)
        {
        }
    }
}

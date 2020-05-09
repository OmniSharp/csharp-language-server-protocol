using System;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    class EnumLikeStringConverter : JsonConverter<IEnumLikeString>
    {
        public override void Write(Utf8JsonWriter writer, IEnumLikeString value, JsonSerializerOptions options)
        {
            new JValue(value.ToString()).WriteTo(writer);
        }

        public override IEnumLikeString ReadJson(JsonReader reader, Type objectType, IEnumLikeString existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            return reader.TokenType switch {
                JsonToken.String => (IEnumLikeString) Activator.CreateInstance(objectType, (string) reader.Value),
                _ => (IEnumLikeString) Activator.CreateInstance(objectType, null)
            };
        }


    }
}

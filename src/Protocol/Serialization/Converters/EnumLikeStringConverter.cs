using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    public class EnumLikeStringConverter : JsonConverter<IEnumLikeString>
    {
        public override IEnumLikeString Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.TokenType switch {
                JsonTokenType.String => (IEnumLikeString) Activator.CreateInstance(typeToConvert, reader.GetString()),
                _ => (IEnumLikeString) Activator.CreateInstance(typeToConvert, null)
            };
        }

        public override void Write(Utf8JsonWriter writer, IEnumLikeString value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value.ToString(), options);
        }
    }
}

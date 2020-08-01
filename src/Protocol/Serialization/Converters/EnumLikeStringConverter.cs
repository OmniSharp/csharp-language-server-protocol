using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    class EnumLikeStringConverter : JsonConverter<IEnumLikeString>
    {
        public override void WriteJson(JsonWriter writer, IEnumLikeString value, JsonSerializer serializer)
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

        public override bool CanRead => true;
    }
}

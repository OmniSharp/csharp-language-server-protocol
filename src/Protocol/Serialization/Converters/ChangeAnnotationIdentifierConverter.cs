using System;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    internal class ChangeAnnotationIdentifierConverter : JsonConverter<ChangeAnnotationIdentifier?>
    {
        public override ChangeAnnotationIdentifier? ReadJson(
            JsonReader reader, Type objectType, ChangeAnnotationIdentifier? existingValue,
            bool hasExistingValue, JsonSerializer serializer
        )
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            if (reader.TokenType == JsonToken.String)
            {
                try
                {
                    return new ChangeAnnotationIdentifier { Identifier = (string) reader.Value };
                }
                catch (ArgumentException ex)
                {
                    throw new JsonSerializationException("Could not deserialize change annotation identifier", ex);
                }
            }

            throw new JsonSerializationException("The JSON value must be a string.");
        }

        public override void WriteJson(JsonWriter writer, ChangeAnnotationIdentifier? value, JsonSerializer serializer)
        {
            if (value is null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteValue(value.Identifier);
        }
    }
}

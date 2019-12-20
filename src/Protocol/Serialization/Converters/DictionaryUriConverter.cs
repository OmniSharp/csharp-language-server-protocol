using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    /// <summary>
    /// This is necessary because Newtonsoft.Json creates <see cref="Uri"/> instances with
    /// <see cref="UriKind.RelativeOrAbsolute"/> which treats UNC paths as relative. NuGet.Core uses
    /// <see cref="UriKind.Absolute"/> which treats UNC paths as absolute. For more details, see:
    /// https://github.com/JamesNK/Newtonsoft.Json/issues/2128
    /// </summary><
    class DictionaryUriConverter<TKey, TValue> :
        JsonConverter<Dictionary<TKey, TValue>> where TKey : Uri
    {
        private readonly AbsoluteUriConverter _uriConverter = new AbsoluteUriConverter();

        public override Dictionary<TKey, TValue> ReadJson(
            JsonReader reader,
            Type objectType,
            Dictionary<TKey, TValue> existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.StartObject)
            {
                throw new JsonException();
            }

            Dictionary<TKey, TValue> value = new Dictionary<TKey, TValue>();

            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndObject)
                {
                    return value;
                }

                // Get the key.
                if (reader.TokenType != JsonToken.PropertyName)
                {
                    throw new JsonException();
                }

                // Get the stringified Uri.
                string propertyName = (string) reader.Value;
                reader.Read();

                Uri key = new Uri(propertyName, UriKind.Absolute);

                // Get the value.
                TValue v = serializer.Deserialize<TValue>(reader);

                // Add to dictionary.
                value.Add((TKey) key, v);
            }

            throw new JsonException();
        }

        public override void WriteJson(
            JsonWriter writer,
            Dictionary<TKey, TValue> value,
            JsonSerializer serializer)
        {
            writer.WriteStartObject();

            foreach (KeyValuePair<TKey, TValue> kvp in value)
            {
                writer.WritePropertyName(AbsoluteUriConverter.Convert(kvp.Key));
                serializer.Serialize(writer, kvp.Value);
            }

            writer.WriteEndObject();
        }
    }
}

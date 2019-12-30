using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    class AbsoluteUriKeyConverter<TValue> : JsonConverter<Dictionary<Uri, TValue>>
    {
        public override Dictionary<Uri, TValue> ReadJson(
            JsonReader reader,
            Type objectType,
            Dictionary<Uri, TValue> existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.StartObject)
            {
                throw new JsonException();
            }

            var dictionary = new Dictionary<Uri, TValue>();

            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndObject)
                {
                    return dictionary;
                }

                // Get the key.
                if (reader.TokenType != JsonToken.PropertyName)
                {
                    throw new JsonSerializationException($"The token type must be a property name. Given {reader.TokenType.ToString()}");
                }

                // Get the stringified Uri.
                var key = new Uri((string)reader.Value, UriKind.RelativeOrAbsolute);
                if (!key.IsAbsoluteUri)
                {
                    throw new JsonSerializationException($"The Uri must be absolute. Given: {reader.Value}");
                }

                // Get the value.
                reader.Read();
                var value = serializer.Deserialize<TValue>(reader);

                // Add to dictionary.
                dictionary.Add(key, value);
            }

            throw new JsonException();
        }

        public override void WriteJson(
            JsonWriter writer,
            Dictionary<Uri, TValue> value,
            JsonSerializer serializer)
        {
            writer.WriteStartObject();

            foreach (var kvp in value)
            {
                var uri = kvp.Key;
                if (!uri.IsAbsoluteUri)
                {
                    throw new JsonSerializationException("The URI value must be an absolute Uri. Relative URI instances are not allowed.");
                }

                if (uri.IsFile)
                {
                    // First add the file scheme and ://
                    var builder = new StringBuilder(uri.Scheme)
                        .Append("://");

                    // UNC file paths use the Host
                    if (uri.HostNameType != UriHostNameType.Basic)
                    {
                        builder.Append(uri.Host);
                    }

                    // Paths that start with a drive letter don't have a slash in the PathAndQuery
                    // but they need it in the final result.
                    if (uri.PathAndQuery[0] != '/')
                    {
                        builder.Append('/');
                    }

                    // Lastly add the remaining parts of the URL
                    builder.Append(uri.PathAndQuery);
                    writer.WritePropertyName(builder.ToString());
                }
                else
                {
                    writer.WritePropertyName(uri.AbsoluteUri);
                }

                serializer.Serialize(writer, kvp.Value);
            }

            writer.WriteEndObject();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    class CompletionListConverter : JsonConverter<CompletionList>
    {
        public override void Write(Utf8JsonWriter writer, CompletionList value, JsonSerializerOptions options)
        {
            if (!value.IsIncomplete)
            {
                  JsonSerializer.Serialize(writer, value.Items.ToArray(), options);
                return;
            }

            writer.WriteStartObject();
            writer.WritePropertyName("isIncomplete");
            writer.WriteValue(value.IsIncomplete);

            writer.WritePropertyName("items");
            writer.WriteStartArray();
            foreach (var item in value.Items)
            {
                  JsonSerializer.Serialize(writer, item, options);
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        public override CompletionList Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                var array = JArray.Load(reader).ToObject<IEnumerable<CompletionItem>>(serializer);
                return new CompletionList(array);
            }

            var result = JObject.Load(reader);
            var items = result["items"].ToObject<IEnumerable<CompletionItem>>(serializer);
            return new CompletionList(items, result["isIncomplete"].Value<bool>());
        }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    class CompletionListConverter : JsonConverter<CompletionList>
    {
        public override void WriteJson(JsonWriter writer, CompletionList value, JsonSerializer serializer)
        {
            if (!value.IsIncomplete)
            {
                serializer.Serialize(writer, value.Items.ToArray());
                return;
            }

            writer.WriteStartObject();
            writer.WritePropertyName("isIncomplete");
            writer.WriteValue(value.IsIncomplete);

            writer.WritePropertyName("items");
            writer.WriteStartArray();
            foreach (var item in value.Items)
            {
                serializer.Serialize(writer, item);
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        public override CompletionList ReadJson(JsonReader reader, Type objectType, CompletionList existingValue, bool hasExistingValue, JsonSerializer serializer)
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

        public override bool CanRead => true;
    }
}

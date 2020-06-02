using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    class CompletionListConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(ICompletionList).IsAssignableFrom(objectType);
        }

        public override void WriteJson(JsonWriter writer, object v, JsonSerializer serializer)
        {
            if (!(v is ICompletionList value)) throw new NotSupportedException("could not convert item to ICompletionList");
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

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Type innerType;
            Type enumerableType;
            Type returnType;
            if (objectType == typeof(CompletionList))
            {
                 innerType = typeof(ResolvedData);
                 enumerableType = typeof(IEnumerable<>).MakeGenericType(typeof(CompletionItem<>).MakeGenericType(innerType));
                 returnType = typeof(CompletionList);
            }
            else
            {
                 innerType = objectType.GetGenericArguments()[0];
                 enumerableType = typeof(IEnumerable<>).MakeGenericType(typeof(CompletionItem<>).MakeGenericType(innerType));
                 returnType = typeof(CompletionList<>).MakeGenericType(innerType);

            }
            if (reader.TokenType == JsonToken.StartArray)
            {
                var array = JArray.Load(reader).ToObject(enumerableType, serializer);
                return Activator.CreateInstance(returnType, array);
            }

            var result = JObject.Load(reader);
            var items = result["items"].ToObject(enumerableType, serializer);
            return Activator.CreateInstance(returnType, new object[] { items, result["isIncomplete"].Value<bool>() });
        }

        public override bool CanRead => true;
        public override bool CanWrite => true;
    }
}

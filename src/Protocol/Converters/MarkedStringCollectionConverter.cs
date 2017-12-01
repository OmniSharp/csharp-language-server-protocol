using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Converters
{
    class MarkedStringCollectionConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var v = (value as MarkedStringContainer).ToArray();
            if (v.Length == 1)
            {
                serializer.Serialize(writer, v[0]);
                return;
            }

            serializer.Serialize(writer, v);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                return new MarkedStringContainer(JArray.Load(reader).ToObject<IEnumerable<MarkedString>>());
            }
            else if (reader.TokenType == JsonToken.StartObject)
            {
                return new MarkedStringContainer(JObject.Load(reader).ToObject<MarkedString>());
            }
            else if (reader.TokenType == JsonToken.String)
            {
                return new MarkedStringContainer(reader.Value as string);
            }

            return new MarkedStringContainer();
        }

        public override bool CanRead => true;

        public override bool CanConvert(Type objectType) => objectType == typeof(MarkedStringContainer);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    class MarkedStringCollectionConverter : JsonConverter<MarkedStringContainer>
    {
        public override void WriteJson(JsonWriter writer, MarkedStringContainer value, JsonSerializer serializer)
        {
            var v = value.ToArray();
            if (v.Length == 1)
            {
                serializer.Serialize(writer, v[0]);
                return;
            }

            serializer.Serialize(writer, v);
        }

        public override MarkedStringContainer ReadJson(JsonReader reader, Type objectType, MarkedStringContainer existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                return new MarkedStringContainer(JArray.Load(reader).ToObject<IEnumerable<MarkedString>>(serializer));
            }
            else if (reader.TokenType == JsonToken.StartObject)
            {
                return new MarkedStringContainer(JObject.Load(reader).ToObject<MarkedString>(serializer));
            }
            else if (reader.TokenType == JsonToken.String)
            {
                return new MarkedStringContainer(reader.Value as string);
            }

            return new MarkedStringContainer();
        }

        public override bool CanRead => true;
    }
}

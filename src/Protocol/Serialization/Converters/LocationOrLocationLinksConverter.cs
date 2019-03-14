using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    class LocationOrLocationLinksConverter : JsonConverter<LocationOrLocationLinks>
    {
        public override void WriteJson(JsonWriter writer, LocationOrLocationLinks value, JsonSerializer serializer)
        {
            var v = value.ToArray();
            if (v.Length == 1 && v[0].IsLocation)
            {
                serializer.Serialize(writer, v[0]);
                return;
            }

            serializer.Serialize(writer, v);
        }

        public override LocationOrLocationLinks ReadJson(JsonReader reader, Type objectType, LocationOrLocationLinks existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                return new LocationOrLocationLinks(JArray.Load(reader).ToObject<IEnumerable<LocationOrLocationLink>>(serializer));
            }
            else if (reader.TokenType == JsonToken.StartObject)
            {
                return new LocationOrLocationLinks(JObject.Load(reader).ToObject<Location>(serializer));
            }

            return new LocationOrLocationLinks();
        }

        public override bool CanRead => true;
    }
}

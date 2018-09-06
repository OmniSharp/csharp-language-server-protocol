using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    class LocationOrLocationsConverter : JsonConverter<LocationOrLocations>
    {
        public override void WriteJson(JsonWriter writer, LocationOrLocations value, JsonSerializer serializer)
        {
            var v = value.ToArray();
            if (v.Length == 1)
            {
                serializer.Serialize(writer, v[0]);
                return;
            }

            serializer.Serialize(writer, v);
        }

        public override LocationOrLocations ReadJson(JsonReader reader, Type objectType, LocationOrLocations existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                return new LocationOrLocations(JArray.Load(reader).ToObject<IEnumerable<Location>>(serializer));
            }
            else if (reader.TokenType == JsonToken.StartObject)
            {
                return new LocationOrLocations(JObject.Load(reader).ToObject<Location>(serializer));
            }

            return new LocationOrLocations();
        }

        public override bool CanRead => true;
    }
}

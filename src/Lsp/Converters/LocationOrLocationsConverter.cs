using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Lsp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Lsp.Converters
{
    class LocationOrLocationsConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var v = (value as LocationOrLocations).ToArray();
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
                return new LocationOrLocations(JArray.Load(reader).ToObject<IEnumerable<Location>>());
            }
            else if (reader.TokenType == JsonToken.StartObject)
            {
                return new LocationOrLocations(JObject.Load(reader).ToObject<Location>());
            }

            return new LocationOrLocations();
        }
        
        public override bool CanRead => true;

        public override bool CanConvert(Type objectType) => objectType == typeof(LocationOrLocations);
    }
}
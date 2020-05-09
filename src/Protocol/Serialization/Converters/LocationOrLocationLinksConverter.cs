using System;
using System.Collections.Generic;
using System.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    class LocationOrLocationLinksConverter : JsonConverter<LocationOrLocationLinks>
    {
        public override void Write(Utf8JsonWriter writer, LocationOrLocationLinks value, JsonSerializerOptions options)
        {
            var v = value.ToArray();
            if (v.Length == 1 && v[0].IsLocation)
            {
                  JsonSerializer.Serialize(writer, v[0], options);
                return;
            }

              JsonSerializer.Serialize(writer, v, options);
        }

        public override LocationOrLocationLinks Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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


    }
}

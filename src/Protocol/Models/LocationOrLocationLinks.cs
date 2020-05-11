using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [JsonConverter(typeof(Converter))]
    public class LocationOrLocationLinks : ContainerBase<LocationOrLocationLink>
    {
        public LocationOrLocationLinks() : this(Enumerable.Empty<LocationOrLocationLink>())
        {
        }

        public LocationOrLocationLinks(IEnumerable<LocationOrLocationLink> items) : base(items)
        {
        }

        public LocationOrLocationLinks(params LocationOrLocationLink[] items) : base(items)
        {
        }

        public static implicit operator LocationOrLocationLinks(LocationOrLocationLink[] items)
        {
            return new LocationOrLocationLinks(items);
        }

        public static implicit operator LocationOrLocationLinks(Collection<LocationOrLocationLink> items)
        {
            return new LocationOrLocationLinks(items);
        }

        public static implicit operator LocationOrLocationLinks(List<LocationOrLocationLink> items)
        {
            return new LocationOrLocationLinks(items);
        }

        class Converter : JsonConverter<LocationOrLocationLinks>
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

            public override LocationOrLocationLinks Read(ref Utf8JsonReader reader, Type typeToConvert,
                JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.StartArray)
                {
                    return JsonSerializer.Deserialize<List<LocationOrLocationLink>>(ref reader, options);
                }

                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    return new LocationOrLocationLinks(JsonSerializer.Deserialize<Location>(ref reader, options));
                }

                return new LocationOrLocationLinks();
            }
        }
    }
}

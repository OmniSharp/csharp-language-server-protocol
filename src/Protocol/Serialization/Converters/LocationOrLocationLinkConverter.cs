using System;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    public class LocationOrLocationLinkConverter : JsonConverter<LocationOrLocationLink>
    {
        public override void Write(Utf8JsonWriter writer, LocationOrLocationLink value, JsonSerializerOptions options)
        {
            if (value.IsLocation)
                  JsonSerializer.Serialize(writer, value.Location, options);
            if (value.IsLocationLink)
                  JsonSerializer.Serialize(writer, value.LocationLink, options);
        }

        public override LocationOrLocationLink Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var obj = JObject.Load(reader);
            if (obj.ContainsKey("uri"))
            {
                return new LocationOrLocationLink(obj.ToObject<Location>());
            }
            else
            {
                return new LocationOrLocationLink(obj.ToObject<LocationLink>());
            }
        }


    }
}

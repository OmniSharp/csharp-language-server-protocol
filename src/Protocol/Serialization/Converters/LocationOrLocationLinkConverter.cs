using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    public class LocationOrLocationLinkConverter : JsonConverter<LocationOrLocationLink>
    {
        public override void WriteJson(JsonWriter writer, LocationOrLocationLink value, JsonSerializer serializer)
        {
            if (value.IsLocation)
                serializer.Serialize(writer, value.Location);
            if (value.IsLocationLink)
                serializer.Serialize(writer, value.LocationLink);
        }

        public override LocationOrLocationLink ReadJson(JsonReader reader, Type objectType, LocationOrLocationLink existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            if (obj.ContainsKey("uri"))
            {
                return new LocationOrLocationLink(obj.ToObject<Location>(serializer));
            }

            return new LocationOrLocationLink(obj.ToObject<LocationLink>(serializer));
        }

        public override bool CanRead => true;
    }
}

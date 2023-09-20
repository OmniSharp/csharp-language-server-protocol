using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

public class LocationOrFileLocationConverter : JsonConverter<LocationOrFileLocation>
{
    public override void WriteJson(JsonWriter writer, LocationOrFileLocation value, JsonSerializer serializer)
    {
        if (value.IsLocation)
            serializer.Serialize(writer, value.Location);
        if (value.IsFileLocation)
            serializer.Serialize(writer, value.FileLocation);
    }

    public override LocationOrFileLocation ReadJson(JsonReader reader, Type objectType, LocationOrFileLocation existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var obj = JObject.Load(reader);
        if (obj.ContainsKey("range"))
        {
            return new LocationOrFileLocation(obj.ToObject<Location>(serializer));
        }

        return new LocationOrFileLocation(obj.ToObject<FileLocation>(serializer));
    }

    public override bool CanRead => true;
}

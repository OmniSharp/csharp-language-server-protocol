using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models;

[JsonConverter(typeof(LocationOrFileLocationConverter))]
public record LocationOrFileLocation
{
    public LocationOrFileLocation(Location location)
    {
        Location = location;
        FileLocation = null;
    }

    public LocationOrFileLocation(FileLocation locationLink)
    {
        Location = null;
        FileLocation = locationLink;
    }

    public bool IsLocation => Location is not null;
    public Location? Location { get; }

    public bool IsFileLocation => FileLocation != null;
    public FileLocation? FileLocation { get; }

    public static implicit operator LocationOrFileLocation(Location location) => new LocationOrFileLocation(location);

    public static implicit operator LocationOrFileLocation(FileLocation locationLink) => new LocationOrFileLocation(locationLink);
}
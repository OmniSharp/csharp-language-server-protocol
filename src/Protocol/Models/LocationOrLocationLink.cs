using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [JsonConverter(typeof(LocationOrLocationLinkConverter))]
    [GenerateContainer("LocationOrLocationLinks")]
    public class LocationOrLocationLink
    {
        public LocationOrLocationLink(Location location)
        {
            Location = location;
            LocationLink = null;
        }

        public LocationOrLocationLink(LocationLink locationLink)
        {
            Location = null;
            LocationLink = locationLink;
        }

        public bool IsLocation => Location is not null;
        public Location? Location { get; }

        public bool IsLocationLink => LocationLink != null;
        public LocationLink? LocationLink { get; }

        public static implicit operator LocationOrLocationLink(Location location) => new LocationOrLocationLink(location);

        public static implicit operator LocationOrLocationLink(LocationLink locationLink) => new LocationOrLocationLink(locationLink);
    }

    [JsonConverter(typeof(LocationOrLocationLinksConverter))]
    public partial class LocationOrLocationLinks { }
}

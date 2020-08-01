using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [JsonConverter(typeof(LocationOrLocationLinkConverter))]
    public struct LocationOrLocationLink {

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

        public bool IsLocation => Location != null;
        public Location Location { get; }

        public bool IsLocationLink => LocationLink != null;
        public LocationLink LocationLink { get; }

        public static implicit operator LocationOrLocationLink(Location location)
        {
            return new LocationOrLocationLink(location);
        }

        public static implicit operator LocationOrLocationLink(LocationLink locationLink)
        {
            return new LocationOrLocationLink(locationLink);
        }
    }
}

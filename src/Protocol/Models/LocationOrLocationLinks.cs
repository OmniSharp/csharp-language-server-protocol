using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [JsonConverter(typeof(LocationOrLocationLinksConverter))]
    public class LocationOrLocationLinks : Container<LocationOrLocationLink>
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
    }
}

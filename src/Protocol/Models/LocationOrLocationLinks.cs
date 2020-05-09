using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
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
    }
}

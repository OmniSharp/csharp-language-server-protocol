using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class LocationContainer : Container<Location>
    {
        public LocationContainer() : this(Enumerable.Empty<Location>())
        {
        }

        public LocationContainer(IEnumerable<Location> items) : base(items)
        {
        }

        public LocationContainer(params Location[] items) : base(items)
        {
        }

        public static implicit operator LocationContainer(Location[] items)
        {
            return new LocationContainer(items);
        }

        public static implicit operator LocationContainer(Collection<Location> items)
        {
            return new LocationContainer(items);
        }

        public static implicit operator LocationContainer(List<Location> items)
        {
            return new LocationContainer(items);
        }
    }
}

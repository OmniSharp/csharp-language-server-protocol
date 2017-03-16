using System.Collections.Generic;
using System.Linq;

namespace Lsp.Models
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
    }
}
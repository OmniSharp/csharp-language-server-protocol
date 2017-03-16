using System.Collections.Generic;
using System.Linq;
using Lsp.Converters;
using Newtonsoft.Json;

namespace Lsp.Models
{
    [JsonConverter(typeof(LocationOrLocationsConverter))]
    public class LocationOrLocations : ContainerBase<Location>
    {
        public LocationOrLocations() : this(Enumerable.Empty<Location>())
        {
        }

        public LocationOrLocations(IEnumerable<Location> items) : base(items)
        {
        }

        public LocationOrLocations(params Location[] items) : base(items)
        {
        }

        public static implicit operator LocationOrLocations(Location[] items)
        {
            return new LocationOrLocations(items);
        }
    }
}
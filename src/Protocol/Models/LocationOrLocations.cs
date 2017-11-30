using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
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

        public static implicit operator LocationOrLocations(Collection<Location> items)
        {
            return new LocationOrLocations(items);
        }

        public static implicit operator LocationOrLocations(List<Location> items)
        {
            return new LocationOrLocations(items);
        }
    }
}
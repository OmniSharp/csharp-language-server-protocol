using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [GenerateContainer]
    public class Location : IEquatable<Location>
    {
        /// <summary>
        /// The uri of the document
        /// </summary>
        public DocumentUri Uri { get; set; } = null!;

        /// <summary>
        /// The range in side the document given by the uri
        /// </summary>
        public Range Range { get; set; } = null!;

        public override bool Equals(object? obj) => Equals(obj as Location);

        public bool Equals(Location? other) =>
            other is not null &&
            DocumentUri.Comparer.Equals(Uri, other.Uri) &&
            EqualityComparer<Range>.Default.Equals(Range, other.Range);

        public override int GetHashCode()
        {
            var hashCode = 1486144663;
            hashCode = hashCode * -1521134295 + DocumentUri.Comparer.GetHashCode(Uri);
            hashCode = hashCode * -1521134295 + EqualityComparer<Range>.Default.GetHashCode(Range);
            return hashCode;
        }

        public static bool operator ==(Location location1, Location location2) => EqualityComparer<Location>.Default.Equals(location1, location2);

        public static bool operator !=(Location location1, Location location2) => !( location1 == location2 );

        private string DebuggerDisplay => $"{{{Range} {Uri}}}";

        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
    }
}

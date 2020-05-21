using System;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class VersionedTextDocumentIdentifier : TextDocumentIdentifier, IEquatable<VersionedTextDocumentIdentifier>
    {
        public bool Equals(VersionedTextDocumentIdentifier other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && Version == other.Version;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((VersionedTextDocumentIdentifier) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ Version.GetHashCode();
            }
        }

        public static bool operator ==(VersionedTextDocumentIdentifier left, VersionedTextDocumentIdentifier right) => Equals(left, right);

        public static bool operator !=(VersionedTextDocumentIdentifier left, VersionedTextDocumentIdentifier right) => !Equals(left, right);

        /// <summary>
        /// The version number of this document.
        /// </summary>
        public long Version { get; set; }
    }
}

using System;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class WorkspaceFolder : IEquatable<WorkspaceFolder>
    {
        /// <summary>
        /// The associated URI for this workspace folder.
        /// </summary>
        public DocumentUri Uri { get; set; }

        /// <summary>
        /// The name of the workspace folder. Defaults to the uri's basename.
        /// </summary>
        public string Name { get; set; }

        public bool Equals(WorkspaceFolder other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Uri.Equals(other.Uri) && Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((WorkspaceFolder) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Uri.GetHashCode() * 397) ^ Name.GetHashCode();
            }
        }

        public static bool operator ==(WorkspaceFolder left, WorkspaceFolder right) => Equals(left, right);

        public static bool operator !=(WorkspaceFolder left, WorkspaceFolder right) => !Equals(left, right);
    }
}

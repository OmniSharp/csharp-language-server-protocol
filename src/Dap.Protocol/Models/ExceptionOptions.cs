using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Models
{
    /// <summary>
    /// ExceptionOptions
    /// An ExceptionOptions assigns configuration options to a set of exceptions.
    /// </summary>
    public class ExceptionOptions
    {
        /// <summary>
        /// A path that selects a single or multiple exceptions in a tree. If 'path' is missing, the whole tree is selected. By convention the first segment of the path is a category that is
        /// used to group exceptions in the UI.
        /// </summary>
        [Optional]
        public Container<ExceptionPathSegment>? Path { get; set; }

        /// <summary>
        /// Condition when a thrown exception should result in a break.
        /// </summary>
        public ExceptionBreakMode BreakMode { get; set; }
    }
}

using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    /// <summary>
    /// Whether the client in general supports change annotations on text edits,
    /// create file, rename file and delete file changes.
    ///
    /// @since 3.16.0
    /// </summary>
    public class WorkspaceEditSupportCapabilitiesChangeAnnotationSupport
    {
        /// <summary>
        /// Whether the client groups edits with equal labels into tree nodes,
        /// for instance all edits labelled with "Changes in Strings" would
        /// be a tree node.
        /// </summary>
        [Optional]
        public bool GroupsOnLabel { get; set; }
    }
}
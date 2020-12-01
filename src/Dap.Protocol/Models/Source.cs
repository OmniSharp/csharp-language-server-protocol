using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Models
{
    /// <summary>
    /// A Source is a descriptor for source code.It is returned from the debug adapter as part of a StackFrame and it is used by clients when specifying breakpoints.
    /// </summary>
    public record Source
    {
        /// <summary>
        /// The short name of the source. Every source returned from the debug adapter has a name. When sending a source to the debug adapter this name is optional.
        /// </summary>
        [Optional]
        public string? Name { get; init; }

        /// <summary>
        /// The path of the source to be shown in the UI. It is only used to locate and load the content of the source if no sourceReference is specified (or its value is 0).
        /// </summary>
        [Optional]
        public string? Path { get; init; }

        /// <summary>
        /// If sourceReference > 0 the contents of the source must be retrieved through the SourceRequest (even if a path is specified). A sourceReference is only valid for a session, so it
        /// must not be used to persist a source.
        /// </summary>
        [Optional]
        public long? SourceReference { get; init; }

        /// <summary>
        /// An optional hint for how to present the source in the UI. A value of 'deemphasize' can be used to indicate that the source is not available or that it is skipped on stepping.
        /// </summary>
        [Optional]
        public SourcePresentationHint? PresentationHint { get; init; }

        /// <summary>
        /// The (optional) origin of this source: possible values 'internal module', 'inlined content from source map', etc.
        /// </summary>
        [Optional]
        public string? Origin { get; init; }

        /// <summary>
        /// An optional list of sources that are related to this source. These may be the source that generated this source.
        /// </summary>
        [Optional]
        public Container<Source>? Sources { get; init; }

        /// <summary>
        /// Optional data that a debug adapter might want to loop through the client. The client should leave the data intact and persist it across sessions. The client should not interpret
        /// the data.
        /// </summary>
        [Optional]
        public JToken? AdapterData { get; init; }

        /// <summary>
        /// The checksums associated with this file.
        /// </summary>
        [Optional]
        public Container<Checksum>? Checksums { get; init; }
    }

    [StringEnum()]
    public readonly partial struct SourcePresentationHint
    {
        public static SourcePresentationHint Normal { get; } = new SourcePresentationHint("normal");
        public static SourcePresentationHint Emphasize { get; } = new SourcePresentationHint("emphasize");
        public static SourcePresentationHint Deemphasize { get; } = new SourcePresentationHint("deemphasize");
    }
}

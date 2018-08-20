using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class FoldingRangeCapability : DynamicCapability
    {
        /// <summary>
        /// The maximum number of folding ranges that the client prefers to receive per document. The value serves as a
        /// hint, servers are free to follow the limit.
        /// </summary>
        [Optional]
        public long? RangeLimit { get; set; }

        /// <summary>
        ///  set, the client signals that it only supports folding complete lines. If set, client will
        /// ignore specified `startCharacter` and `endCharacter` properties in a FoldingRange.
        /// </summary>
        [Optional]
        public bool LineFoldingOnly { get; set; }
    }
}

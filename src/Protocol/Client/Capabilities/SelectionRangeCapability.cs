using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class SelectionRangeCapability : DynamicCapability, ConnectedCapability<ISelectionRangeHandler>
    {
        /// <summary>
        /// The maximum number of folding ranges that the client prefers to receive per document. The value serves as a
        /// hint, servers are free to follow the limit.
        /// </summary>
        [Optional]
        public int? RangeLimit { get; set; }

        /// <summary>
        /// If set, the client signals that it only supports folding complete lines. If set, client will
        /// ignore specified `startCharacter` and `endCharacter` properties in a FoldingRange.
        /// </summary>
        public bool LineFoldingOnly { get; set; }
    }
}

using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class ColorProviderCapability : DynamicCapability, ConnectedCapability<IDocumentColorHandler>, ConnectedCapability<IColorPresentationHandler> { }
    public class FoldingRangeCapability : DynamicCapability, ConnectedCapability<IFoldingRangeHandler>, ConnectedCapability<IFoldingRangeHandler>
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

using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class WorkspaceSymbolInformation : ISymbolInformation
    {
        /// <summary>
        /// The name of this symbol.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The kind of this symbol.
        /// </summary>
        public SymbolKind Kind { get; set; }

        /// <summary>
        /// The location of this symbol.
        /// </summary>
        public Location Location { get; set; }

        /// <summary>
        /// The name of the symbol containing this symbol.
        /// </summary>
        [Optional]
        public string ContainerName { get; set; }
    }
}

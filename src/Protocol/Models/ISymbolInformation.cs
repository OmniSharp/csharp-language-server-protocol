using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Represents information about programming constructs like variables, classes,
    /// public classs etc.
    /// </summary>
    public interface ISymbolInformation
    {
        /// <summary>
        /// The name of this symbol.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The kind of this symbol.
        /// </summary>
        SymbolKind Kind { get; set; }

        /// <summary>
        /// The location of this symbol.
        /// </summary>
        Location Location { get; set; }

        /// <summary>
        /// The name of the symbol containing this symbol.
        /// </summary>
        [Optional]
        string ContainerName { get; set; }
    }
}

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Represents information about programming constructs like variables, classes,
    /// public classs etc.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class SymbolInformation
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
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ContainerName { get; set; }
    }
}
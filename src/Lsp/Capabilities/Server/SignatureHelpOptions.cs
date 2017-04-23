using System.Collections.Generic;
using Lsp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lsp.Capabilities.Server
{
    /// <summary>
    ///  Signature help options.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class SignatureHelpOptions : ISignatureHelpOptions
    {
        /// <summary>
        ///  The characters that trigger signature help
        ///  automatically.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Container<string> TriggerCharacters { get; set; }
    }
}
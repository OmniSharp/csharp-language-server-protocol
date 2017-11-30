using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Models;

namespace OmniSharp.Extensions.LanguageServer.Capabilities.Server
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

        public static SignatureHelpOptions Of(ISignatureHelpOptions options)
        {
            return new SignatureHelpOptions() { TriggerCharacters = options.TriggerCharacters };
        }
    }
}

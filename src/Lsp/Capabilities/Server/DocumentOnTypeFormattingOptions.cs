using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OmniSharp.Extensions.LanguageServerProtocol.Models;

namespace OmniSharp.Extensions.LanguageServerProtocol.Capabilities.Server
{
    /// <summary>
    ///  Format document on type options
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class DocumentOnTypeFormattingOptions : IDocumentOnTypeFormattingOptions
    {
        /// <summary>
        ///  A character on which formatting should be triggered, like `}`.
        /// </summary>
        public string FirstTriggerCharacter { get; set; }

        /// <summary>
        ///  More trigger characters.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Container<string> MoreTriggerCharacter { get; set; }

        public static DocumentOnTypeFormattingOptions Of(IDocumentOnTypeFormattingOptions options)
        {
            return new DocumentOnTypeFormattingOptions() {
                FirstTriggerCharacter = options.FirstTriggerCharacter,
                MoreTriggerCharacter = options.MoreTriggerCharacter
            };
        }
    }
}
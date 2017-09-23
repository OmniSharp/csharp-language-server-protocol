using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OmniSharp.Extensions.LanguageServerProtocol.Models;

namespace OmniSharp.Extensions.LanguageServerProtocol.Capabilities.Server
{
    /// <summary>
    ///  Completion options.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class CompletionOptions : ICompletionOptions
    {
        /// <summary>
        ///  The server provides support to resolve additional
        ///  information for a completion item.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool ResolveProvider { get; set; }

        /// <summary>
        ///  The characters that trigger completion automatically.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Container<string> TriggerCharacters { get; set; }

        public static CompletionOptions Of(ICompletionOptions options)
        {
            return new CompletionOptions() { ResolveProvider = options.ResolveProvider, TriggerCharacters = options.TriggerCharacters };
        }
    }
}
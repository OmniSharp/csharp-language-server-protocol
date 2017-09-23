using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class TextDocumentSaveRegistrationOptions : TextDocumentRegistrationOptions
    {
        /// <summary>
        ///  The client is supposed to include the content on save.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool IncludeText { get; set; }
    }
}
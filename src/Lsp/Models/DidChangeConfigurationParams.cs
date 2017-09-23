using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServerProtocol.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class DidChangeConfigurationParams
    {
        /// <summary>
        ///  The actual changed settings
        /// </summary>
        public IDictionary<string, BooleanNumberString> Settings { get; set; } = new Dictionary<string, BooleanNumberString>();
    }
}
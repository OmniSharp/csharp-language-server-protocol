using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServerProtocol.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class Location
    {
        /// <summary>
        /// The uri of the document
        /// </summary>
        public Uri Uri { get; set; }

        /// <summary>
        /// The range in side the document given by the uri
        /// </summary>
        public Range Range { get; set; }
    }
}
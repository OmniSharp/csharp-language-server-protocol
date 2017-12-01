using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    ///  An event describing a file change.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class FileEvent
    {
        /// <summary>
        ///  The file's URI.
        /// </summary>
        public Uri Uri { get; set; }
        /// <summary>
        ///  The change type.
        /// </summary>
        public FileChangeType Type { get; set; }
    }
}
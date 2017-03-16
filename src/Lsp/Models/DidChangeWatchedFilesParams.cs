using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lsp.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class DidChangeWatchedFilesParams
    {
        /// <summary>
        ///  The actual file events.
        /// </summary>
        public Container<FileEvent> Changes { get; set; }
    }
}
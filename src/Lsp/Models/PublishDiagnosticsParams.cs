using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lsp.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class PublishDiagnosticsParams
    {
        /// <summary>
        ///  The URI for which diagnostic information is reported.
        /// </summary>
        public Uri Uri { get; set; }

        /// <summary>
        ///  An array of diagnostic information items.
        /// </summary>
        public Container<Diagnostic> Diagnostics { get; set; }
    }
}
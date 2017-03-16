using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lsp.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class WorkspaceEdit
    {
        /// <summary>
        /// Holds changes to existing resources.
        /// </summary>
        public IDictionary<Uri, IEnumerable<TextEdit>> Changes { get; set; } = new Dictionary<Uri, IEnumerable<TextEdit>>();
    }
}
using System;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class WorkspaceFolder
    {
        /// <summary>
        /// The associated URI for this workspace folder.
        /// </summary>
        [JsonConverter(typeof(AbsoluteUriConverter))]
        public Uri Uri { get; set; }

        /// <summary>
        /// The name of the workspace folder. Defaults to the uri's basename.
        /// </summary>
        public string Name { get; set; }
    }
}

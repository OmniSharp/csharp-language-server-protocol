using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Common interface for types that support resolution.
    /// </summary>
    public interface ICanBeResolved<out T>
        where T : CanBeResolvedData
    {
        /// <summary>
        /// A data entry field that is preserved for resolve requests~
        /// </summary>
        [Optional]
        T Data { get; }
    }

    public class CanBeResolvedData
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "$$___handler___$$", NullValueHandling = NullValueHandling.Ignore)]
        internal Guid? handler;
        [JsonExtensionData]
        internal IDictionary<string, JToken> data { get; set; } = new Dictionary<string, JToken>();
    }

    public sealed class ResolvedData : CanBeResolvedData
    {
        [JsonIgnore]
        public IDictionary<string, JToken> Data
        {
            get => data;
            set => data = value;
        }
    }
}

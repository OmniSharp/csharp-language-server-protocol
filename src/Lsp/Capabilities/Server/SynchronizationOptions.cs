using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Models;

namespace OmniSharp.Extensions.LanguageServer.Capabilities.Server
{
    /// <summary>
    ///  Signature help options.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class SynchronizationOptions : ISynchronizationOptions
    {
        /// <summary>
        /// The client supports sending will save notifications.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool WillSave { get; set; }

        /// <summary>
        /// The client supports sending a will save request and
        /// waits for a response providing text edits which will
        /// be applied to the document before it is saved.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool WillSaveWaitUntil { get; set; }

        /// <summary>
        /// The client supports did save notifications.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool DidSave { get; set; }

        public static SynchronizationOptions Of(ISynchronizationOptions options)
        {
            return new SynchronizationOptions() { WillSave = options.WillSave, DidSave = options.DidSave, WillSaveWaitUntil = options.WillSaveWaitUntil };
        }
    }
}
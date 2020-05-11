using System.Text.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    /// <summary>
    ///  Signature help options.
    /// </summary>
    public class SynchronizationOptions : ISynchronizationOptions
    {
        /// <summary>
        /// The client supports sending will save notifications.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public bool WillSave { get; set; }

        /// <summary>
        /// The client supports sending a will save request and
        /// waits for a response providing text edits which will
        /// be applied to the document before it is saved.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public bool WillSaveWaitUntil { get; set; }

        /// <summary>
        /// The client supports did save notifications.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public bool DidSave { get; set; }

        public static SynchronizationOptions Of(ISynchronizationOptions options)
        {
            return new SynchronizationOptions() { WillSave = options.WillSave, DidSave = options.DidSave, WillSaveWaitUntil = options.WillSaveWaitUntil };
        }
    }
}

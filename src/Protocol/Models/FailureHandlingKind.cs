using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FailureHandlingKind
    {
        /// <summary>
        /// All operations are executed transactional. That means they either all
        /// succeed or no changes at all are applied to the workspace.
        /// </summary>
        [EnumMember(Value = "abort")]
        Abort,
        /// <summary>
        /// All operations are executed transactional. That means they either all
        /// succeed or no changes at all are applied to the workspace.
        /// </summary>
        [EnumMember(Value = "transactional")]
        Transactional,
        /// <summary>
        ///Supports deleting existing files and folders.
        /// </summary>
        [EnumMember(Value = "delete")]
        Delete,
    }
}

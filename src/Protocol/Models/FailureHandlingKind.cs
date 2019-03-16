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
        /// <summary>
        /// If the workspace edit contains only textual file changes they are executed transactional.
        /// If resource changes (create, rename or delete file) are part of the change the failure
        /// handling strategy is abort.
        /// </summary>
        [EnumMember(Value = "textOnlyTransactional")]
        TextOnlyTransactional,
    }
}

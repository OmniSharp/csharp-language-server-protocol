using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FailureHandlingKind
    {
        /// <summary>
        /// Applying the workspace change is simply aborted if one of the changes provided
        /// fails. All operations executed before the failing operation stay executed.
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
        /// If the workspace edit contains only textual file changes they are executed transactional.
        /// If resource changes (create, rename or delete file) are part of the change the failure
        /// handling strategy is abort.
        /// </summary>
        [EnumMember(Value = "textOnlyTransactional")]
        TextOnlyTransactional,
        /// <summary>
        /// The client tries to undo the operations already executed. But there is no
        /// guarantee that this is succeeding.
        /// </summary>
        [EnumMember(Value = "undo")]
        Undo,
    }
}

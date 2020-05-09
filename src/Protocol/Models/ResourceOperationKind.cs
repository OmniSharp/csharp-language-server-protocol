using System.Runtime.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ResourceOperationKind
    {
        /// <summary>
        ///Supports creating new files and folders.
        /// </summary>
        [EnumMember(Value = "create")]
        Create,
        /// <summary>
        ///Supports renaming existing files and folders.
        /// </summary>
        [EnumMember(Value = "rename")]
        Rename,
        /// <summary>
        ///Supports deleting existing files and folders.
        /// </summary>
        [EnumMember(Value = "delete")]
        Delete,
    }
}

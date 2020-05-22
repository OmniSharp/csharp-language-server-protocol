using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// How a signature help was triggered.
    ///
    /// @since 3.15.0
    /// </summary>
    [JsonConverter(typeof(NumberEnumConverter))]
    public enum SignatureHelpTriggerKind
    {
        /// <summary>
        /// Signature help was invoked manually by the user or by a command.
        /// </summary>
        Invoked = 1,
        /// <summary>
        /// Signature help was triggered by a trigger character.
        /// </summary>
        TriggerCharacter = 2,
        /// <summary>
        /// Signature help was triggered by the cursor moving or by the document content changing.
        /// </summary>
        ContentChange = 3,
    }
}

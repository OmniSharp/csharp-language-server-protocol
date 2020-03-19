using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public interface ISignatureHelpOptions : IWorkDoneProgressOptions
    {
        /// <summary>
        /// The characters that trigger signature help
        /// automatically.
        /// </summary>
        [Optional]
        Container<string> TriggerCharacters { get; set; }

        /// <summary>
        /// List of characters that re-trigger signature help.
        ///
        /// These trigger characters are only active when signature help is already showing. All trigger characters
        /// are also counted as re-trigger characters.
        ///
        /// @since 3.15.0
        /// </summary>
        [Optional]
        Container<string> RetriggerCharacters { get; set; }
    }
}

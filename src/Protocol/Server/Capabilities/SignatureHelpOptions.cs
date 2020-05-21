using System.Collections.Generic;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    /// <summary>
    ///  Signature help options.
    /// </summary>
    public class SignatureHelpOptions : WorkDoneProgressOptions, ISignatureHelpOptions
    {
        /// <summary>
        /// The characters that trigger signature help
        /// automatically.
        /// </summary>
        [Optional]
        public Container<string> TriggerCharacters { get; set; }

        /// <summary>
        /// List of characters that re-trigger signature help.
        ///
        /// These trigger characters are only active when signature help is already showing. All trigger characters
        /// are also counted as re-trigger characters.
        ///
        /// @since 3.15.0
        /// </summary>
        [Optional]
        public Container<string> RetriggerCharacters { get; set; }

        public static SignatureHelpOptions Of(ISignatureHelpOptions options, IEnumerable<IHandlerDescriptor> descriptors)
        {
            return new SignatureHelpOptions() { TriggerCharacters = options.TriggerCharacters, RetriggerCharacters = options.RetriggerCharacters, WorkDoneProgress = options.WorkDoneProgress };
        }
    }
}

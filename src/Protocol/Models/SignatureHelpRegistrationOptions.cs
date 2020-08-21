using System.Collections.Generic;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class SignatureHelpRegistrationOptions : WorkDoneTextDocumentRegistrationOptions
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

        /// <summary>
        /// Signature help options.
        /// </summary>
        public class StaticOptions : WorkDoneProgressOptions
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
        }

        class SignatureHelpRegistrationOptionsConverter : RegistrationOptionsConverterBase<SignatureHelpRegistrationOptions, StaticOptions>
        {
            public SignatureHelpRegistrationOptionsConverter() : base(nameof(ServerCapabilities.SignatureHelpProvider))
            {
            }
            public override StaticOptions Convert(SignatureHelpRegistrationOptions source) => new StaticOptions {
                TriggerCharacters = source.TriggerCharacters,
                RetriggerCharacters = source.RetriggerCharacters,
                WorkDoneProgress = source.WorkDoneProgress
            };
        }
    }
}

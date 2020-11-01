using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class DocumentOnTypeFormattingRegistrationOptions : WorkDoneTextDocumentRegistrationOptions
    {
        /// <summary>
        /// A character on which formatting should be triggered, like `}`.
        /// </summary>
        public string FirstTriggerCharacter { get; set; } = null!;

        /// <summary>
        /// More trigger characters.
        /// </summary>
        [Optional]
        public Container<string>? MoreTriggerCharacter { get; set; }

        /// <summary>
        /// Format document on type options
        /// </summary>
        public class StaticOptions : WorkDoneProgressOptions
        {
            /// <summary>
            /// A character on which formatting should be triggered, like `}`.
            /// </summary>
            public string FirstTriggerCharacter { get; set; } = null!;

            /// <summary>
            /// More trigger characters.
            /// </summary>
            [Optional]
        public Container<string>? MoreTriggerCharacter { get; set; }
        }

        class DocumentOnTypeFormattingRegistrationOptionsConverter : RegistrationOptionsConverterBase<DocumentOnTypeFormattingRegistrationOptions, StaticOptions>
        {

            public DocumentOnTypeFormattingRegistrationOptionsConverter() : base(nameof(ServerCapabilities.DocumentOnTypeFormattingProvider))
            {
            }

            public override StaticOptions Convert(DocumentOnTypeFormattingRegistrationOptions source) =>
                new StaticOptions {
                    FirstTriggerCharacter = source.FirstTriggerCharacter,
                    MoreTriggerCharacter = source.MoreTriggerCharacter,
                    WorkDoneProgress = source.WorkDoneProgress,
                };
        }
    }
}

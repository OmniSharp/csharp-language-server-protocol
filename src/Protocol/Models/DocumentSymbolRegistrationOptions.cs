using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class DocumentSymbolRegistrationOptions : WorkDoneTextDocumentRegistrationOptions
    {
        /// <summary>
        /// A human-readable string that is shown when multiple outlines trees
        /// are shown for the same document.
        ///
        /// @since 3.16.0 - proposed state
        /// </summary>
        [Optional]
        public string? Label { get; set; }

        public class StaticOptions : WorkDoneProgressOptions
        {
            /// <summary>
            /// A human-readable string that is shown when multiple outlines trees
            /// are shown for the same document.
            ///
            /// @since 3.16.0 - proposed state
            /// </summary>
            [Optional]
            public string? Label { get; set; }
        }

        class DocumentSymbolRegistrationOptionsConverter : RegistrationOptionsConverterBase<DocumentSymbolRegistrationOptions, StaticOptions>
        {
            public DocumentSymbolRegistrationOptionsConverter() : base(nameof(ServerCapabilities.DocumentSymbolProvider))
            {
            }

            public override StaticOptions Convert(DocumentSymbolRegistrationOptions source) =>
                new StaticOptions { Label = source.Label, WorkDoneProgress = source.WorkDoneProgress };
        }
    }
}

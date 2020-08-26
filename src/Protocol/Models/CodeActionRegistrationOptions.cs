using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class CodeActionRegistrationOptions : WorkDoneTextDocumentRegistrationOptions
    {
        /// <summary>
        /// CodeActionKinds that this server may return.
        ///
        /// The list of kinds may be generic, such as `CodeActionKind.Refactor`, or the server
        /// may list out every specific kind they provide.
        /// </summary>
        [Optional]
        public Container<CodeActionKind>? CodeActionKinds { get; set; } = new Container<CodeActionKind>();

        public class StaticOptions : WorkDoneProgressOptions
        {
            /// <summary>
            /// CodeActionKinds that this server may return.
            ///
            /// The list of kinds may be generic, such as `CodeActionKind.Refactor`, or the server
            /// may list out every specific kind they provide.
            /// </summary>
            [Optional]
            public Container<CodeActionKind>? CodeActionKinds { get; set; } = new Container<CodeActionKind>();
        }

        class CodeActionRegistrationOptionsConverter : RegistrationOptionsConverterBase<CodeActionRegistrationOptions, StaticOptions>
        {
            public CodeActionRegistrationOptionsConverter() : base(nameof(ServerCapabilities.CodeActionProvider))
            {
            }

            public override StaticOptions Convert(CodeActionRegistrationOptions source)
            {
                return new StaticOptions {
                    CodeActionKinds = source.CodeActionKinds,
                    WorkDoneProgress = source.WorkDoneProgress,
                };
            }
        }
    }
}

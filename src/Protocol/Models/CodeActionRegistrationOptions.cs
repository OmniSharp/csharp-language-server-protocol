using System.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
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

        /// <summary>
        /// The server provides support to resolve additional
        /// information for a code action.
        ///
        /// @since 3.16.0
        /// </summary>
        public bool ResolveProvider { get; set; }

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

            /// <summary>
            /// The server provides support to resolve additional
            /// information for a code action.
            ///
            /// @since 3.16.0
            /// </summary>
            [Optional]
            public bool ResolveProvider { get; set; }
        }

        class CodeActionRegistrationOptionsConverter : RegistrationOptionsConverterBase<CodeActionRegistrationOptions, StaticOptions>
        {
            private readonly IHandlersManager _handlersManager;

            public CodeActionRegistrationOptionsConverter(IHandlersManager handlersManager) : base(nameof(ServerCapabilities.CodeActionProvider))
            {
                _handlersManager = handlersManager;
            }

            public override StaticOptions Convert(CodeActionRegistrationOptions source)
            {
                return new StaticOptions {
                    CodeActionKinds = source.CodeActionKinds,
                    ResolveProvider = source.ResolveProvider || _handlersManager.Descriptors.Any(z => z.HandlerType == typeof(ICodeActionResolveHandler)),
                    WorkDoneProgress = source.WorkDoneProgress,
                };
            }
        }
    }
}

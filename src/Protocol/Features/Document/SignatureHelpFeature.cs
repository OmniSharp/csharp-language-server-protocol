using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    namespace Models
    {
        [Parallel]
        [Method(TextDocumentNames.SignatureHelp, Direction.ClientToServer)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))
        ]
        [RegistrationOptions(typeof(SignatureHelpRegistrationOptions)), Capability(typeof(SignatureHelpCapability))]
        public partial class SignatureHelpParams : TextDocumentPositionParams, IWorkDoneProgressParams, IRequest<SignatureHelp?>
        {
            /// <summary>
            /// The signature help context. This is only available if the client specifies
            /// to send this using the client capability  `textDocument.signatureHelp.contextSupport === true`
            ///
            /// @since 3.15.0
            /// </summary>
            public SignatureHelpContext Context { get; set; } = null!;
        }

        /// <summary>
        /// Signature help represents the signature of something
        /// callable. There can be multiple signature but only one
        /// active and only one active parameter.
        /// </summary>
        public partial class SignatureHelp
        {
            /// <summary>
            /// One or more signatures.
            /// </summary>
            public Container<SignatureInformation> Signatures { get; set; } = new Container<SignatureInformation>();

            /// <summary>
            /// The active signature.
            /// </summary>
            [Optional]
            public int? ActiveSignature { get; set; }

            /// <summary>
            /// The active parameter of the active signature.
            /// </summary>
            [Optional]
            public int? ActiveParameter { get; set; }
        }

        [GenerateRegistrationOptions(nameof(ServerCapabilities.SignatureHelpProvider))]
        public partial class SignatureHelpRegistrationOptions : ITextDocumentRegistrationOptions, IWorkDoneProgressOptions
        {
            /// <summary>
            /// The characters that trigger signature help
            /// automatically.
            /// </summary>
            [Optional]
            public Container<string>? TriggerCharacters { get; set; }

            /// <summary>
            /// List of characters that re-trigger signature help.
            ///
            /// These trigger characters are only active when signature help is already showing. All trigger characters
            /// are also counted as re-trigger characters.
            ///
            /// @since 3.15.0
            /// </summary>
            [Optional]
            public Container<string>? RetriggerCharacters { get; set; }
        }
    }

    namespace Client.Capabilities
    {
        public partial class SignatureHelpCapability : DynamicCapability, ConnectedCapability<ISignatureHelpHandler>
        {
            /// <summary>
            /// The client supports the following `SignatureInformation`
            /// specific properties.
            /// </summary>
            [Optional]
            public SignatureInformationCapabilityOptions? SignatureInformation { get; set; }

            /// <summary>
            /// The client supports to send additional context information for a
            /// `textDocument/signatureHelp` request. A client that opts into
            /// contextSupport will also support the `retriggerCharacters` on
            /// `StaticOptions`.
            ///
            /// @since 3.15.0
            /// </summary>
            [Optional]
            public bool ContextSupport { get; set; }
        }

        public class SignatureInformationCapabilityOptions
        {
            /// <summary>
            /// Client supports the follow content formats for the content property. The order describes the preferred format of the client.
            /// </summary>
            [Optional]
            public Container<MarkupKind>? DocumentationFormat { get; set; }

            [Optional] public SignatureParameterInformationCapabilityOptions? ParameterInformation { get; set; }

            /// <summary>
            /// The client support the `activeParameter` property on `SignatureInformation`
            /// literal.
            ///
            /// @since 3.16.0 - proposed state
            /// </summary>
            [Optional]
            public bool ActiveParameterSupport { get; set; }
        }

        public class SignatureParameterInformationCapabilityOptions
        {
            /// <summary>
            /// The client supports processing label offsets instead of a
            /// simple label string.
            /// </summary>
            [Optional]
            public bool LabelOffsetSupport { get; set; }
        }
    }

    namespace Document
    {
    }
}

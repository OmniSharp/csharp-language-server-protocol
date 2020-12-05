using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;
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
        public partial record SignatureHelpParams : TextDocumentPositionParams, IWorkDoneProgressParams, IRequest<SignatureHelp?>
        {
            /// <summary>
            /// The signature help context. This is only available if the client specifies
            /// to send this using the client capability  `textDocument.signatureHelp.contextSupport === true`
            ///
            /// @since 3.15.0
            /// </summary>
            public SignatureHelpContext Context { get; init; }
        }

        /// <summary>
        /// Additional information about the context in which a signature help request was triggered.
        ///
        /// @since 3.15.0
        /// </summary>
        public record SignatureHelpContext
        {
            /// <summary>
            /// Action that caused signature help to be triggered.
            /// </summary>
            public SignatureHelpTriggerKind TriggerKind { get; init; }

            /// <summary>
            /// Character that caused signature help to be triggered.
            ///
            /// This is undefined when `triggerKind !== SignatureHelpTriggerKind.TriggerCharacter`
            /// </summary>
            [Optional]
            public string? TriggerCharacter { get; init; }

            /// <summary>
            /// `true` if signature help was already showing when it was triggered.
            ///
            /// Retriggers occur when the signature help is already active and can be caused by actions such as
            /// typing a trigger character, a cursor move, or document content changes.
            /// </summary>
            public bool IsRetrigger { get; init; }

            /// <summary>
            /// The currently active `SignatureHelp`.
            ///
            /// The `activeSignatureHelp` has its `SignatureHelp.activeSignature` field updated based on
            /// the user navigating through available signatures.
            /// </summary>
            [Optional]
            public SignatureHelp? ActiveSignatureHelp { get; init; }
        }

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

        /// <summary>
        /// Signature help represents the signature of something
        /// callable. There can be multiple signature but only one
        /// active and only one active parameter.
        /// </summary>
        public partial record SignatureHelp
        {
            /// <summary>
            /// One or more signatures.
            /// </summary>
            public Container<SignatureInformation> Signatures { get; init; } = new();

            /// <summary>
            /// The active signature.
            /// </summary>
            [Optional]
            public int? ActiveSignature { get; init; }

            /// <summary>
            /// The active parameter of the active signature.
            /// </summary>
            [Optional]
            public int? ActiveParameter { get; init; }
        }

        /// <summary>
        /// Represents the signature of something callable. A signature
        /// can have a label, like a function-name, a doc-comment, and
        /// a set of parameters.
        /// </summary>
        [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
        public record SignatureInformation
        {
            /// <summary>
            /// The label of this signature. Will be shown in
            /// the UI.
            /// </summary>
            public string Label { get; init; }

            /// <summary>
            /// The human-readable doc-comment of this signature. Will be shown
            /// in the UI but can be omitted.
            /// </summary>
            [Optional]
            public StringOrMarkupContent? Documentation { get; init; }

            /// <summary>
            /// The parameters of this signature.
            /// </summary>
            [Optional]
            public Container<ParameterInformation>? Parameters { get; init; }

            /// <summary>
            /// The index of the active parameter.
            ///
            /// If provided, this is used in place of `SignatureHelp.activeParameter`.
            ///
            /// @since 3.16.0 - proposed state
            /// </summary>
            [Optional]
            public int? ActiveParameter { get; init; }

            private string DebuggerDisplay => $"{Label}{Documentation?.ToString() ?? ""}";

            /// <inheritdoc />
            public override string ToString() => DebuggerDisplay;
        }

        /// <summary>
        /// Represents a parameter of a callable-signature. A parameter can
        /// have a label and a doc-comment.
        /// </summary>
        [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
        public record ParameterInformation
        {
            /// <summary>
            /// The label of this parameter. Will be shown in
            /// the UI.
            /// </summary>
            public ParameterInformationLabel Label { get; init; }

            /// <summary>
            /// The human-readable doc-comment of this parameter. Will be shown
            /// in the UI but can be omitted.
            /// </summary>
            [Optional]
            public StringOrMarkupContent? Documentation { get; init; }

            private string DebuggerDisplay => $"{Label}{( Documentation != null ? $" {Documentation}" : string.Empty )}";

            /// <inheritdoc />
            public override string ToString() => DebuggerDisplay;
        }

        [JsonConverter(typeof(ParameterInformationLabelConverter))]
        [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
        public record ParameterInformationLabel
        {
            public ParameterInformationLabel((int start, int end) range) => Range = range;

            public ParameterInformationLabel(string label) => Label = label;

            public (int start, int end) Range { get; }
            public bool IsRange => Label == null;
            public string? Label { get; }
            public bool IsLabel => Label != null;

            public static implicit operator ParameterInformationLabel(string label) => new ParameterInformationLabel(label);

            public static implicit operator ParameterInformationLabel((int start, int end) range) => new ParameterInformationLabel(range);

            private string DebuggerDisplay => IsRange ? $"(start: {Range.start}, end: {Range.end})" : IsLabel ? Label! : string.Empty;

            /// <inheritdoc />
            public override string ToString() => DebuggerDisplay;
        }

        [GenerateRegistrationOptions(nameof(ServerCapabilities.SignatureHelpProvider))]
        [RegistrationName(TextDocumentNames.SignatureHelp)]
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

using System.Diagnostics;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    namespace Models
    {
        [Parallel]
        [Method(TextDocumentNames.DocumentColor, Direction.ClientToServer)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document")]
        [GenerateHandlerMethods]
        [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
        [RegistrationOptions(typeof(DocumentColorRegistrationOptions))]
        [Capability(typeof(ColorProviderCapability))]
        public partial record DocumentColorParams : IPartialItemsRequest<Container<ColorInformation>, ColorInformation>, IWorkDoneProgressParams
        {
            /// <summary>
            /// The text document.
            /// </summary>
            public TextDocumentIdentifier TextDocument { get; init; }
        }

        public partial record ColorInformation
        {
            /// <summary>
            /// The range in the document where this color appears.
            /// </summary>
            public Range Range { get; init; }

            /// <summary>
            /// The actual color value for this color range.
            /// </summary>
            public DocumentColor Color { get; init; }

            public ColorPresentationParams For(TextDocumentIdentifier textDocumentIdentifier)
            {
                return new ColorPresentationParams
                {
                    Color = Color,
                    Range = Range,
                    TextDocument = textDocumentIdentifier
                };
            }
        }

        [Parallel]
        [Method(TextDocumentNames.ColorPresentation, Direction.ClientToServer)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document")]
        [GenerateHandlerMethods]
        [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
        [Capability(typeof(ColorProviderCapability))]
        public partial record ColorPresentationParams : ColorInformation, IRequest<Container<ColorPresentation>>, IDoesNotParticipateInRegistration
        {
            /// <summary>
            /// The document to provide document links for.
            /// </summary>
            public TextDocumentIdentifier TextDocument { get; init; } = null!;
        }

        public partial record ColorPresentation
        {
            /// <summary>
            /// The label of this color presentation. It will be shown on the color
            /// picker header. By default this is also the text that is inserted when selecting
            /// this color presentation.
            /// </summary>
            public string Label { get; init; } = null!;

            /// <summary>
            /// An [edit](#TextEdit) which is applied to a document when selecting
            /// this presentation for the color.  When `falsy` the [label](#ColorPresentation.label)
            /// is used.
            /// </summary>
            [Optional]
            public TextEdit? TextEdit { get; init; }

            /// <summary>
            /// An optional array of additional [text edits](#TextEdit) that are applied when
            /// selecting this color presentation. Edits must not overlap with the main [edit](#ColorPresentation.textEdit) nor with themselves.
            /// </summary>
            [Optional]
            public TextEditContainer? AdditionalTextEdits { get; init; }
        }

        /// <summary>
        /// Represents a color in RGBA space.
        /// </summary>
        [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
        public record DocumentColor
        {
            /// <summary>
            /// The red component of this color in the range [0-1].
            /// </summary>
            public double Red { get; init; }

            /// <summary>
            /// The green component of this color in the range [0-1].
            /// </summary>
            public double Green { get; init; }

            /// <summary>
            /// The blue component of this color in the range [0-1].
            /// </summary>
            public double Blue { get; init; }

            /// <summary>
            /// The alpha component of this color in the range [0-1].
            /// </summary>
            public double Alpha { get; init; }

            private string DebuggerDisplay => $"R:{Red} G:{Green} B:{Blue} A:{Alpha}";

            /// <inheritdoc />
            public override string ToString()
            {
                return DebuggerDisplay;
            }
        }

        [GenerateRegistrationOptions(nameof(ServerCapabilities.ColorProvider))]
        [RegistrationName(TextDocumentNames.DocumentColor)]
        public partial class DocumentColorRegistrationOptions : IWorkDoneProgressOptions, ITextDocumentRegistrationOptions, IStaticRegistrationOptions
        {
        }
    }

    namespace Client.Capabilities
    {
        [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(TextDocumentClientCapabilities.ColorProvider))]
        public partial class ColorProviderCapability : DynamicCapability
        {
        }
    }

    namespace Document
    {
    }
}

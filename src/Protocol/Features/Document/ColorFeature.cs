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
        [Method(TextDocumentNames.ColorPresentation, Direction.ClientToServer)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))
        ]
        [RegistrationOptions(typeof(DocumentColorRegistrationOptions)), Capability(typeof(ColorProviderCapability))]
        public partial class ColorPresentationParams : IRequest<Container<ColorPresentation>>
        {
            /// <summary>
            /// The document to provide document links for.
            /// </summary>
            public TextDocumentIdentifier TextDocument { get; set; } = null!;

            /// <summary>
            /// The actual color value for this color range.
            /// </summary>
            public DocumentColor Color { get; set; } = null!;

            /// <summary>
            /// The range in the document where this color appers.
            /// </summary>
            public Range Range { get; set; } = null!;
        }

        [Parallel]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))
        ]
        [RegistrationOptions(typeof(DocumentColorRegistrationOptions)), Capability(typeof(ColorProviderCapability))]
        public partial class ColorPresentation
        {
            /// <summary>
            /// The label of this color presentation. It will be shown on the color
            /// picker header. By default this is also the text that is inserted when selecting
            /// this color presentation.
            /// </summary>
            public string Label { get; set; } = null!;

            /// <summary>
            /// An [edit](#TextEdit) which is applied to a document when selecting
            /// this presentation for the color.  When `falsy` the [label](#ColorPresentation.label)
            /// is used.
            /// </summary>
            [Optional]
            public TextEdit? TextEdit { get; set; }

            /// <summary>
            /// An optional array of additional [text edits](#TextEdit) that are applied when
            /// selecting this color presentation. Edits must not overlap with the main [edit](#ColorPresentation.textEdit) nor with themselves.
            /// </summary>
            [Optional]
            public TextEditContainer? AdditionalTextEdits { get; set; }
        }

        [Parallel]
        [Method(TextDocumentNames.DocumentColor, Direction.ClientToServer)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))
        ]
        [RegistrationOptions(typeof(DocumentColorRegistrationOptions)), Capability(typeof(ColorProviderCapability))]
        public partial  class DocumentColorParams : IPartialItemsRequest<Container<ColorInformation>, ColorInformation>, IWorkDoneProgressParams
        {
            /// <summary>
            /// The text document.
            /// </summary>
            public TextDocumentIdentifier TextDocument { get; set; } = null!;
        }

        public partial class ColorInformation
        {
            /// <summary>
            /// The range in the document where this color appers.
            /// </summary>
            public Range Range { get; set; } = null!;

            /// <summary>
            /// The actual color value for this color range.
            /// </summary>
            public DocumentColor Color { get; set; } = null!;
        }

        [GenerateRegistrationOptions(nameof(ServerCapabilities.ColorProvider))]
        public partial class DocumentColorRegistrationOptions : IWorkDoneProgressOptions, ITextDocumentRegistrationOptions, IStaticRegistrationOptions
        {
        }
    }

    namespace Client.Capabilities
    {
        [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(TextDocumentClientCapabilities.ColorProvider))]
        public partial class ColorProviderCapability : DynamicCapability, ConnectedCapability<IDocumentColorHandler>, ConnectedCapability<IColorPresentationHandler>
        {
        }
    }

    namespace Document
    {
    }
}

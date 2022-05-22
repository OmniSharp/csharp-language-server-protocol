using System.Diagnostics;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        [Method(TextDocumentNames.InlayHint, Direction.ClientToServer)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document", Name = "InlayHints"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))
        ]
        [RegistrationOptions(typeof(InlayHintRegistrationOptions)), Capability(typeof(InlayHintWorkspaceClientCapabilities))]
        public partial record InlayHintParams : ITextDocumentIdentifierParams, IWorkDoneProgressParams,
                                                IRequest<Container<InlayHint>?>
        {
            /// <summary>
            /// The text document.
            /// </summary>
            public TextDocumentIdentifier TextDocument { get; init; }

            /// <summary>
            /// The visible document range for which inlay hints should be computed.
            /// </summary>
            public Range Range { get; init; }
        }

        /// <summary>
        /// Inlay hint information.
        ///
        /// @since 3.17.0
        /// </summary>
        [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
        [Parallel]
        [Method(TextDocumentNames.InlayHintResolve, Direction.ClientToServer)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document", Name = "InlayHintResolve")]
        [GenerateHandlerMethods]
        [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
        [GenerateTypedData]
        [Capability(typeof(InlayHintWorkspaceClientCapabilities))]
        public partial record InlayHint : ICanBeResolved, IRequest<InlayHint>
        {
            /// <summary>
            /// The position of this hint.
            /// </summary>
            public Position Position { get; init; }

            /// <summary>
            /// The label of this hint. A human readable string or an array of
            /// InlayHintLabelPart label parts.
            ///
            /// *Note* that neither the string nor the label part can be empty.
            /// </summary>
            public StringOrInlayHintLabelParts Label { get; init; }

            /// <summary>
            /// The kind of this hint. Can be omitted in which case the client
            /// should fall back to a reasonable default.
            /// </summary>
            public InlayHintKind? Kind { get; init; }

            /// <summary>
            /// Optional text edits that are performed when accepting this inlay hint.
            ///
            /// *Note* that edits are expected to change the document so that the inlay
            /// hint (or its nearest variant) is now part of the document and the inlay
            /// hint itself is now obsolete.
            ///
            /// Depending on the client capability `inlayHint.resolveSupport` clients
            /// might resolve this property late using the resolve request.
            /// </summary>
            [Optional]
            public Container<TextEdit>? TextEdits { get; init; }

            /// <summary>
            /// The tooltip text when you hover over this item.
            ///
            /// Depending on the client capability `inlayHint.resolveSupport` clients
            /// might resolve this property late using the resolve request.
            /// </summary>
            [Optional]
            public StringOrMarkupContent? Tooltip { get; init; }

            /// <summary>
            /// Render padding before the hint.
            ///
            /// Note: Padding should use the editor's background color, not the
            /// background color of the hint itself. That means padding can be used
            /// to visually align/separate an inlay hint.
            /// </summary>
            [Optional]
            public bool? PaddingLeft { get; init; }

            /// <summary>
            /// Render padding after the hint.
            ///
            /// Note: Padding should use the editor's background color, not the
            /// background color of the hint itself. That means padding can be used
            /// to visually align/separate an inlay hint.
            /// </summary>
            [Optional]
            public bool? PaddingRight { get; init; }

            /// <summary>
            /// A data entry field that is preserved on a document link between a
            /// DocumentLinkRequest and a DocumentLinkResolveRequest.
            /// </summary>
            [Optional]
            public JToken? Data { get; init; }

            private string DebuggerDisplay => ToString();
        }

        /// <summary>
        /// An inlay hint label part allows for interactive and composite labels
        /// of inlay hints.
        ///
        /// @since 3.17.0
        /// </summary>
        [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
        public partial record InlayHintLabelPart
        {
            /// <summary>
            /// The value of this label part.
            /// </summary>
            public string Value { get; init; }

            /// <summary>
            /// The tooltip text when you hover over this label part. Depending on
            /// the client capability `inlayHint.resolveSupport` clients might resolve
            /// this property late using the resolve request.
            /// </summary>
            [Optional]
            public StringOrMarkupContent? Tooltip { get; init; }

            /// <summary>
            /// An optional source code location that represents this
            /// label part.
            ///
            /// The editor will use this location for the hover and for code navigation
            /// features: This part will become a clickable link that resolves to the
            /// definition of the symbol at the given location (not necessarily the
            /// location itself), it shows the hover that shows at the given location,
            /// and it shows a context menu with further code navigation commands.
            ///
            /// Depending on the client capability `inlayHint.resolveSupport` clients
            /// might resolve this property late using the resolve request.
            /// </summary>
            [Optional]
            public Location? Location { get; init; }

            /// <summary>
            /// An optional command for this label part.
            ///
            /// Depending on the client capability `inlayHint.resolveSupport` clients
            /// might resolve this property late using the resolve request.
            /// </summary>
            [Optional]
            public Command? Command { get; init; }

            private string DebuggerDisplay => ToString();
        }

        [JsonConverter(typeof(Converter))]
        [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
        public record StringOrInlayHintLabelParts
        {
            public StringOrInlayHintLabelParts(string value) => String = value;

            public StringOrInlayHintLabelParts(IEnumerable<InlayHintLabelPart> inlayHintLabelParts) => InlayHintLabelParts = new(inlayHintLabelParts);

            public string? String { get; }
            public bool HasString => InlayHintLabelParts is null;
            public Container<InlayHintLabelPart>? InlayHintLabelParts { get; }
            public bool HasInlayHintLabelParts => InlayHintLabelParts is { };

            public static implicit operator StringOrInlayHintLabelParts?(string? value) => value is null ? null : new StringOrInlayHintLabelParts(value);

            public static implicit operator StringOrInlayHintLabelParts?(MarkupContent? markupContent) =>
                markupContent is null ? null : new StringOrInlayHintLabelParts(markupContent);

            private string DebuggerDisplay =>
                $"{( HasString ? String : HasInlayHintLabelParts ? string.Join(", ", InlayHintLabelParts!.Select(z => z.ToString())) : string.Empty )}";

            /// <inheritdoc />
            public override string ToString() => DebuggerDisplay;

            internal class Converter : JsonConverter<StringOrInlayHintLabelParts>
            {
                public override void WriteJson(JsonWriter writer, StringOrInlayHintLabelParts value, JsonSerializer serializer)
                {
                    if (value.HasString)
                    {
                        writer.WriteValue(value.String);
                    }
                    else
                    {
                        serializer.Serialize(writer, value.InlayHintLabelParts ?? Array.Empty<InlayHintLabelPart>());
                    }
                }

                public override StringOrInlayHintLabelParts ReadJson(
                    JsonReader reader, Type objectType, StringOrInlayHintLabelParts existingValue, bool hasExistingValue, JsonSerializer serializer
                )
                {
                    if (reader.TokenType == JsonToken.StartArray)
                    {
                        var result = JArray.Load(reader);
                        return new StringOrInlayHintLabelParts(result.ToObject<Container<InlayHintLabelPart>>());
                    }

                    if (reader.TokenType == JsonToken.String)
                    {
                        return new StringOrInlayHintLabelParts(( reader.Value as string )!);
                    }

                    return "";
                }

                public override bool CanRead => true;
            }
        }

        /// <summary>
        /// Inlay hint kinds.
        ///
        /// @since 3.17.0
        /// </summary>
        /// 
        [JsonConverter(typeof(NumberEnumConverter))]
        public enum InlayHintKind
        {
            /// <summary>
            /// An inlay hint that for a type annotation.
            /// </summary>
            Type = 1,

            /// <summary>
            /// An inlay hint that is for a parameter.
            /// </summary>
            Parameter = 2
        }

        [GenerateRegistrationOptions(nameof(ServerCapabilities.InlayHintProvider))]
        [RegistrationOptionsConverter(typeof(InlayHintRegistrationOptionsConverter))]
        [RegistrationName(TextDocumentNames.InlayHint)]
        public partial class InlayHintRegistrationOptions : ITextDocumentRegistrationOptions, IWorkDoneProgressOptions
        {
            /// <summary>
            /// The server provides support to resolve additional
            /// information for a code action.
            ///
            /// @since 3.16.0
            /// </summary>
            [Optional]
            public bool ResolveProvider { get; set; }

            private class InlayHintRegistrationOptionsConverter : RegistrationOptionsConverterBase<InlayHintRegistrationOptions, StaticOptions>
            {
                private readonly IHandlersManager _handlersManager;

                public InlayHintRegistrationOptionsConverter(IHandlersManager handlersManager)
                {
                    _handlersManager = handlersManager;
                }

                public override StaticOptions Convert(InlayHintRegistrationOptions source)
                {
                    return new()
                    {
                        ResolveProvider = source.ResolveProvider || _handlersManager.Descriptors.Any(z => z.HandlerType == typeof(IInlayHintResolveHandler)),
                        WorkDoneProgress = source.WorkDoneProgress,
                    };
                }
            }
        }
    }

    namespace Client.Capabilities
    {
        [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(TextDocumentClientCapabilities.InlayHint))]
        public partial class InlayHintWorkspaceClientCapabilities : DynamicCapability
        {
            /// <summary>
            /// Indicates which properties a client can resolve lazily on a inlay
            /// hint.
            /// </summary>
            [Optional]
            public InlayHintCapabilityResolveSupport? ResolveSupport { get; set; }
        }

        /// <summary>
        /// Indicates which properties a client can resolve lazily on a inlay
        /// hint.
        /// </summary>
        public class InlayHintCapabilityResolveSupport
        {
            /// <summary>
            /// The properties that a client can resolve lazily.
            /// </summary>
            public Container<string> Properties { get; set; }
        }
    }

    namespace Document
    {
    }
}

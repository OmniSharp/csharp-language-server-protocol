using MediatR;
using Newtonsoft.Json;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    namespace Models
    {
        [Parallel]
        [Method(TextDocumentNames.Rename, Direction.ClientToServer)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))
        ]
        [RegistrationOptions(typeof(RenameRegistrationOptions)), Capability(typeof(RenameCapability))]
        public partial record RenameParams : ITextDocumentIdentifierParams, IRequest<WorkspaceEdit?>, IWorkDoneProgressParams
        {
            /// <summary>
            /// The document to format.
            /// </summary>
            public TextDocumentIdentifier TextDocument { get; init; }

            /// <summary>
            /// The position at which this request was sent.
            /// </summary>
            public Position Position { get; init; }

            /// <summary>
            /// The new name of the symbol. If the given name is not valid the
            /// request must return a [ResponseError](#ResponseError) with an
            /// appropriate message set.
            /// </summary>
            public string NewName { get; init; }
        }

        [Parallel]
        [Method(TextDocumentNames.PrepareRename, Direction.ClientToServer)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))
        ]
        [RegistrationOptions(typeof(RenameRegistrationOptions)), Capability(typeof(RenameCapability))]
        public partial record PrepareRenameParams : TextDocumentPositionParams, IRequest<RangeOrPlaceholderRange?>
        {
        }

        [JsonConverter(typeof(RangeOrPlaceholderRangeConverter))]
        public record RangeOrPlaceholderRange
        {
            private RenameDefaultBehavior? _renameDefaultBehavior;
            private Range? _range;
            private PlaceholderRange? _placeholderRange;

            public RangeOrPlaceholderRange(Range value)
            {
                _range = value;
            }

            public RangeOrPlaceholderRange(PlaceholderRange value)
            {
                _placeholderRange = value;
            }

            public RangeOrPlaceholderRange(RenameDefaultBehavior renameDefaultBehavior)
            {
                _renameDefaultBehavior = renameDefaultBehavior;
            }

            public bool IsPlaceholderRange => _placeholderRange != null;

            public PlaceholderRange? PlaceholderRange
            {
                get => _placeholderRange;
                init {
                    _placeholderRange = value;
                    _renameDefaultBehavior = default;
                    _range = null;
                }
            }

            public bool IsRange => _range is not null;

            public Range? Range
            {
                get => _range;
                init {
                    _placeholderRange = default;
                    _renameDefaultBehavior = default;
                    _range = value;
                }
            }

            public bool IsDefaultBehavior => _renameDefaultBehavior is not null;

            public RenameDefaultBehavior? DefaultBehavior
            {
                get => _renameDefaultBehavior;
                init {
                    _placeholderRange = default;
                    _renameDefaultBehavior = value;
                    _range = default;
                }
            }

            public object? RawValue
            {
                get {
                    if (IsPlaceholderRange) return PlaceholderRange;
                    if (IsRange) return Range;
                    if (IsDefaultBehavior) return DefaultBehavior;
                    return default;
                }
            }

            public static implicit operator RangeOrPlaceholderRange(PlaceholderRange value) => new RangeOrPlaceholderRange(value);

            public static implicit operator RangeOrPlaceholderRange(Range value) => new RangeOrPlaceholderRange(value);
        }

        public record PlaceholderRange
        {
            public Range Range { get; init; }
            public string Placeholder { get; init; }
        }

        public record RenameDefaultBehavior
        {
            public bool DefaultBehavior { get; init; }
        }

        [GenerateRegistrationOptions(nameof(ServerCapabilities.RenameProvider))]
        [RegistrationName(TextDocumentNames.Rename)]
        public partial class RenameRegistrationOptions : ITextDocumentRegistrationOptions, IWorkDoneProgressOptions, IStaticRegistrationOptions
        {
            /// <summary>
            /// Renames should be checked and tested before being executed.
            /// </summary>
            [Optional]
            public bool PrepareProvider { get; set; }
        }
    }

    namespace Client.Capabilities
    {
        [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(TextDocumentClientCapabilities.Rename))]
        public class RenameCapability : DynamicCapability
        {
            /// <summary>
            /// Client supports testing for validity of rename operations
            /// before execution.
            /// </summary>
            [Optional]
            public bool PrepareSupport { get; set; }

            /// <summary>
            /// Client supports the default behavior result (`{ defaultBehavior: boolean }`).
            ///
            /// @since version 3.16.0
            /// </summary>
            [Optional]
            public PrepareSupportDefaultBehavior PrepareSupportDefaultBehavior { get; set; }

            /// <summary>
            /// Whether th client honors the change annotations in
            /// text edits and resource operations returned via the
            /// `CodeAction#edit` property by for example presenting
            /// the workspace edit in the user interface and asking
            /// for confirmation.
            ///
            /// @since 3.16.0 - proposed state
            /// </summary>
            [Optional]
            public bool HonorsChangeAnnotations { get; set; }
        }


        public enum PrepareSupportDefaultBehavior
        {
            /// <summary>
            /// The client's default behavior is to select the identifier
            /// according the to language's syntax rule.
            /// </summary>
            Identifier = 1
        }
    }

    namespace Document
    {
    }
}

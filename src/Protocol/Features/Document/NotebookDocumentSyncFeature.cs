using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Minimatch;
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
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    namespace Models
    {
        /// <summary>
        /// The params sent in a open notebook document notification.
        ///
        /// @since 3.17.0
        /// </summary>
        [Serial]
        [Method(NotebookDocumentNames.DidOpen, Direction.ClientToServer)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document")]
        [GenerateHandlerMethods]
        [GenerateRequestMethods(typeof(INotebookDocumentLanguageClient), typeof(ILanguageClient))]
        [RegistrationOptions(typeof(NotebookDocumentSyncOptions))]
        [Capability(typeof(NotebookDocumentSyncClientCapabilities))]
        public partial class DidOpenNotebookDocumentParams : IRequest
        {
            /// <summary>
            /// The notebook document that got opened.
            /// </summary>
            public NotebookDocument NotebookDocument { get; set; }

            /// <summary>
            /// The text documents that represent the content
            /// of a notebook cell.
            /// </summary>
            public Container<TextDocumentItem> CellTextDocuments { get; set; }
        }

        /// <summary>
        /// The params sent in a change notebook document notification.
        ///
        /// @since 3.17.0
        /// </summary>
        [Serial]
        [Method(NotebookDocumentNames.DidChange, Direction.ClientToServer)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document")]
        [GenerateHandlerMethods]
        [GenerateRequestMethods(typeof(INotebookDocumentLanguageClient), typeof(ILanguageClient))]
        [RegistrationOptions(typeof(NotebookDocumentSyncOptions))]
        [Capability(typeof(NotebookDocumentSyncClientCapabilities))]
        public partial record DidChangeNotebookDocumentParams : IRequest
        {
            /// <summary>
            /// The notebook document that did change. The version number points
            /// to the version after all provided changes have been applied.
            /// </summary>
            public VersionedNotebookDocumentIdentifier NotebookDocument { get; set; }

            /// <summary>
            /// The actual changes to the notebook document.
            ///
            /// The change describes single state change to the notebook document.
            /// So it moves a notebook document, its cells and its cell text document
            /// contents from state S to S'.
            ///
            /// To mirror the content of a notebook using change events use the
            /// following approach:
            /// - start with the same initial content
            /// - apply the 'notebookDocument/didChange' notifications in the order
            ///   you receive them.
            /// </summary>
            public NotebookDocumentChangeEvent Change { get; set; }
        }

        [Serial]
        [Method(NotebookDocumentNames.DidSave, Direction.ClientToServer)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document")]
        [GenerateHandlerMethods]
        [GenerateRequestMethods(typeof(INotebookDocumentLanguageClient), typeof(ILanguageClient))]
        [RegistrationOptions(typeof(NotebookDocumentSyncOptions))]
        [Capability(typeof(NotebookDocumentSyncClientCapabilities))]
        public partial class DidSaveNotebookDocumentParams : INotebookDocumentIdentifierParams, IRequest
        {
            /// <summary>
            /// The notebook document that got saved.
            /// </summary>
            public NotebookDocumentIdentifier NotebookDocument { get; set; }
        }

        [Parallel]
        [Method(NotebookDocumentNames.DidClose, Direction.ClientToServer)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document")]
        [GenerateHandlerMethods]
        [GenerateRequestMethods(typeof(INotebookDocumentLanguageClient), typeof(ILanguageClient))]
        [RegistrationOptions(typeof(NotebookDocumentSyncOptions))]
        [Capability(typeof(NotebookDocumentSyncClientCapabilities))]
        public partial class DidCloseNotebookDocumentParams : INotebookDocumentIdentifierParams, IRequest
        {
            /// <summary>
            /// The notebook document that got closed.
            /// </summary>
            public NotebookDocumentIdentifier NotebookDocument { get; set; }

            /// <summary>
            /// The text documents that represent the content
            /// of a notebook cell that got closed.
            /// </summary>
            public Container<TextDocumentIdentifier> CellTextDocuments { get; set; }
        }

        /// <summary>
        /// A change event for a notebook document.
        ///
        /// @since 3.17.0
        /// </summary>
        public partial record NotebookDocumentChangeEvent
        {
            /// <summary>
            /// The changed meta data if any.
            /// </summary>
            public JObject? Metadata { get; init; }

            /// <summary>
            /// Changes to cells
            /// </summary>
            public NotebookDocumentChangeEventCells Cells { get; init; }
        }

        /// <summary>
        /// Changes to cells
        /// </summary>
        public partial record NotebookDocumentChangeEventCells
        {
            /// <summary>
            /// Changes to the cell structure to add or
            /// remove cells.
            /// </summary>
            public NotebookDocumentChangeEventCellsStructure? Structure { get; init; }

            /// <summary>
            /// Changes to notebook cells properties like its
            /// kind, execution summary or metadata.
            /// </summary>
            public Container<NotebookCell>? Data { get; set; }

            /// <summary>
            /// Changes to the text content of notebook cells.
            /// </summary>
            public NotebookDocumentChangeEventCellsTextContent TextContent { get; init; }
        }

        /// <summary>
        /// Changes to the cell structure to add or
        /// remove cells.
        /// </summary>
        public partial record NotebookDocumentChangeEventCellsStructure
        {
            /// <summary>
            /// The change to the cell array.
            /// </summary>
            public NotebookCellArrayChange Array { get; set; }

            /// <summary>
            /// Additional opened cell text documents.
            /// </summary>
            public Container<TextDocumentItem>? DidOpen { get; set; }

            /// <summary>
            /// Additional closed cell text documents.
            /// </summary>
            public Container<TextDocumentIdentifier>? DidClose { get; set; }
        }

        /// <summary>
        /// A change describing how to move a `NotebookCell`
        /// array from state S to S'.
        ///
        /// @since 3.17.0
        /// </summary>
        public partial record NotebookCellArrayChange
        {
            /// <summary>
            /// The start oftest of the cell that changed.
            /// </summary>
            public uint Start { get; set; }

            /// <summary>
            /// The deleted cells
            /// </summary>
            public uint DeleteCount { get; set; }

            /// <summary>
            /// The new cells, if any
            /// </summary>
            public Container<NotebookCell>? Cells { get; set; }
        }

        /// <summary>
        /// Changes to the text content of notebook cells.
        /// </summary>
        public partial record NotebookDocumentChangeEventCellsTextContent
        {
            public VersionedTextDocumentIdentifier Document { get; set; }
            public Container<TextDocumentContentChangeEvent> Changes { get; set; }
        }

        /// <summary>
        /// An event describing a change to a text document. If range and rangeLength are omitted
        /// the new text is considered to be the full content of the document.
        /// </summary>
        public record NotebookDocumentContentChangeEvent
        {
            /// <summary>
            /// The range of the document that changed.
            /// </summary>
            [Optional]
            public Range? Range { get; init; }

            /// <summary>
            /// The length of the range that got replaced.
            /// </summary>
            /// <remarks>
            /// <see cref="uint" /> in the LSP spec
            /// </remarks>
            [Optional]
            public int RangeLength { get; init; }

            /// <summary>
            /// The new text of the document.
            /// </summary>
            public string Text { get; init; } = null!;
        }

        /// <summary>
        /// A notebook document.
        ///
        /// @since 3.17.0
        /// </summary>
        public partial record NotebookDocument
        {
            /// <summary>
            /// The notebook document's uri.
            /// </summary>
            public DocumentUri Uri { get; set; }

            /// <summary>
            /// The type of the notebook.
            /// </summary>
            public string NotebookType { get; set; }

            /// <summary>
            /// The version number of this document (it will increase after each
            /// change, including undo/redo).
            /// </summary>
            public int Version { get; set; }

            /// <summary>
            /// Additional metadata stored with the notebook
            /// document.
            /// </summary>
            public JObject? Metadata { get; set; }

            /// <summary>
            /// The cells of a notebook.
            /// </summary>
            public Container<NotebookCell> Cells { get; set; }
        }


        /// <summary>
        /// A notebook cell.
        ///
        /// A cell's document URI must be unique across ALL notebook
        /// cells and can therefore be used to uniquely identify a
        /// notebook cell or the cell's text document.
        ///
        /// @since 3.17.0
        /// </summary>
        public partial record NotebookCell
        {
            /// <summary>
            /// The cell's kind
            /// </summary>
            public NotebookCellKind Kind { get; set; }

            /// <summary>
            /// The URI of the cell's text document
            /// content.
            /// </summary>
            public DocumentUri Document { get; set; }

            /// <summary>
            /// Additional metadata stored with the cell.
            /// </summary>
            public JObject? Metadata { get; set; }

            /// <summary>
            /// Additional execution summary information
            /// if supported by the client.
            /// </summary>
            public ExecutionSummary? ExecutionSummary { get; set; }
        }

        public partial record ExecutionSummary
        {
            /// <summary>
            /// A strict monotonically increasing value
            /// indicating the execution order of a cell
            /// inside a notebook.
            /// </summary>
            public uint ExecutionOrder { get; set; }

            /// <summary>
            /// Whether the execution was successful or
            /// not if known by the client.
            /// </summary>
            public bool? Success { get; set; }
        }

        [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
        public record NotebookCellTextDocumentFilter
        {
            /// <summary>
            /// A filter that matches against the notebook
            /// containing the notebook cell. If a string
            /// value is provided it matches against the
            /// notebook type. '*' matches every notebook.
            /// </summary>
            public StringOrNotebookDocumentFilter Notebook { get; init; }

            /// <summary>
            /// A language id like `python`.
            ///
            /// Will be matched against the language id of the
            /// notebook cell document. '*' matches every language.
            /// </summary>
            [Optional]
            public string? Language { get; init; }

            private string DebuggerDisplay =>
                $"{Notebook} {Language}";

            /// <inheritdoc />
            public override string ToString() => DebuggerDisplay;
        }

        [JsonConverter(typeof(Converter))]
        [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
        public record StringOrNotebookDocumentFilter
        {
            public StringOrNotebookDocumentFilter(string value) => String = value;

            public StringOrNotebookDocumentFilter(NotebookDocumentFilter notebookDocumentFilter) => NotebookDocumentFilter = notebookDocumentFilter;

            public string? String { get; }
            public bool HasString => NotebookDocumentFilter is null;
            public NotebookDocumentFilter? NotebookDocumentFilter { get; }
            public bool HasNotebookDocumentFilter => NotebookDocumentFilter is { };

            public static implicit operator StringOrNotebookDocumentFilter?(string? value) => value is null ? null : new StringOrNotebookDocumentFilter(value);

            public static implicit operator StringOrNotebookDocumentFilter?(NotebookDocumentFilter notebookDocumentFilter) =>
                notebookDocumentFilter is null ? null : new StringOrNotebookDocumentFilter(notebookDocumentFilter);

            private string DebuggerDisplay =>
                $"{( HasString ? String : HasNotebookDocumentFilter ? NotebookDocumentFilter : string.Empty )}";

            /// <inheritdoc />
            public override string ToString() => DebuggerDisplay;

            internal class Converter : JsonConverter<StringOrNotebookDocumentFilter>
            {
                public override void WriteJson(JsonWriter writer, StringOrNotebookDocumentFilter value, JsonSerializer serializer)
                {
                    if (value.HasString)
                    {
                        writer.WriteValue(value.String);
                    }
                    else
                    {
                        serializer.Serialize(writer, value.NotebookDocumentFilter);
                    }
                }

                public override StringOrNotebookDocumentFilter ReadJson(
                    JsonReader reader, Type objectType, StringOrNotebookDocumentFilter existingValue, bool hasExistingValue, JsonSerializer serializer
                )
                {
                    if (reader.TokenType == JsonToken.StartObject)
                    {
                        return new StringOrNotebookDocumentFilter(JObject.Load(reader).ToObject<NotebookDocumentFilter>(serializer));
                    }

                    if (reader.TokenType == JsonToken.String)
                    {
                        return new StringOrNotebookDocumentFilter(( reader.Value as string )!);
                    }

                    return new StringOrNotebookDocumentFilter("");
                }

                public override bool CanRead => true;
            }
        }

        [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
        public class NotebookDocumentFilter : IEquatable<NotebookDocumentFilter>
        {
            public static NotebookDocumentFilter ForPattern(string wildcard) => new NotebookDocumentFilter { Pattern = wildcard };

            public static NotebookDocumentFilter ForNotebookType(string notebookType) => new NotebookDocumentFilter { NotebookType = notebookType };

            public static NotebookDocumentFilter ForScheme(string scheme) => new NotebookDocumentFilter { Scheme = scheme };

            /// <summary>
            /// A language id, like `typescript`.
            /// </summary>
            [Optional]
            public string? NotebookType { get; init; }

            /// <summary>
            /// does the document filter contains a language
            /// </summary>
            [JsonIgnore]
            public bool HasNotebookType => NotebookType != null;

            /// <summary>
            /// A Uri [scheme](#Uri.scheme), like `file` or `untitled`.
            /// </summary>
            [Optional]
            public string? Scheme { get; init; }

            /// <summary>
            /// does the document filter contains a scheme
            /// </summary>
            [JsonIgnore]
            public bool HasScheme => Scheme != null;

            /// <summary>
            /// A glob pattern, like `*.{ts,js}`.
            /// </summary>
            [Optional]
            public string? Pattern
            {
                get => _pattern;
                init
                {
                    _pattern = value;
                    _minimatcher = new Minimatcher(value!, new Options { MatchBase = true });
                }
            }

            /// <summary>
            /// does the document filter contains a paattern
            /// </summary>
            [JsonIgnore]
            public bool HasPattern => Pattern != null;

            private string? _pattern;
            private Minimatcher? _minimatcher;

            public static explicit operator string(NotebookDocumentFilter notebookDocumentFilter)
            {
                var items = new List<string>();
                if (notebookDocumentFilter.HasNotebookType)
                {
                    items.Add(notebookDocumentFilter.NotebookType!);
                }

                if (notebookDocumentFilter.HasScheme)
                {
                    items.Add(notebookDocumentFilter.Scheme!);
                }

                if (notebookDocumentFilter.HasPattern)
                {
                    items.Add(notebookDocumentFilter.Pattern!);
                }

                return $"[{string.Join(", ", items)}]";
            }

            public bool IsMatch(NotebookDocumentAttributes attributes)
            {
                if (HasNotebookType && HasPattern && HasScheme)
                {
                    return NotebookType == attributes.NotebookType && Scheme == attributes.Scheme && _minimatcher?.IsMatch(attributes.Uri.ToString()) == true;
                }

                if (HasNotebookType && HasPattern)
                {
                    return NotebookType == attributes.NotebookType && _minimatcher?.IsMatch(attributes.Uri.ToString()) == true;
                }

                if (HasNotebookType && HasScheme)
                {
                    return NotebookType == attributes.NotebookType && Scheme == attributes.Scheme;
                }

                if (HasPattern && HasScheme)
                {
                    return Scheme == attributes.Scheme && _minimatcher?.IsMatch(attributes.Uri.ToString()) == true;
                }

                if (HasNotebookType)
                {
                    return NotebookType == attributes.NotebookType;
                }

                if (HasScheme)
                {
                    return Scheme == attributes.Scheme;
                }

                if (HasPattern)
                {
                    return _minimatcher?.IsMatch(attributes.Uri.ToString()) == true;
                }

                return false;
            }

            public bool Equals(NotebookDocumentFilter? other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return _pattern == other._pattern && NotebookType == other.NotebookType && Scheme == other.Scheme;
            }

            public override bool Equals(object? obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != GetType()) return false;
                return Equals((NotebookDocumentFilter)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = _pattern != null ? _pattern.GetHashCode() : 0;
                    hashCode = ( hashCode * 397 ) ^ ( NotebookType != null ? NotebookType.GetHashCode() : 0 );
                    hashCode = ( hashCode * 397 ) ^ ( Scheme != null ? Scheme.GetHashCode() : 0 );
                    return hashCode;
                }
            }

            public static bool operator ==(NotebookDocumentFilter left, NotebookDocumentFilter right) => Equals(left, right);

            public static bool operator !=(NotebookDocumentFilter left, NotebookDocumentFilter right) => !Equals(left, right);

            private string DebuggerDisplay => (string)this;

            /// <inheritdoc />
            public override string ToString() => DebuggerDisplay;
        }

        public record NotebookSelectorCell
        {
            public string Language { get; init; }
        }

        /// <summary>
        /// The notebooks to be synced
        /// </summary>
        [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
        public class NotebookSelector : IEquatable<NotebookSelector>
        {
            /// <summary>
            /// The notebook to be synced If a string
            /// value is provided it matches against the
            /// notebook type. '*' matches every notebook.
            /// </summary>
            public StringOrNotebookDocumentFilter? Notebook { get; init; }

            public bool HasNotebook => Notebook is not null;

            /// <summary>
            /// The cells of the matching notebook to be synced.
            /// </summary>
            public Container<NotebookSelectorCell>? Cells { get; init; }

            public bool HasCells => Cells is not null;

            public bool IsMatch(NotebookDocumentAttributes attributes)
            {
                return Notebook?.NotebookDocumentFilter?.IsMatch(attributes) == true
                    || ( attributes.Language is not null && Cells?.Any(z => z.Language == attributes.Language) == true );
            }

            private string DebuggerDisplay => (string)this;

            /// <inheritdoc />
            public override string ToString() => DebuggerDisplay;

            public static explicit operator string(NotebookSelector notebookDocumentFilter)
            {
                var items = new List<string>();
                if (notebookDocumentFilter.HasNotebook)
                {
                    if (notebookDocumentFilter.Notebook.HasString)
                    {
                        items.Add(notebookDocumentFilter.Notebook.String);
                    }
                    else if (notebookDocumentFilter.Notebook.HasNotebookDocumentFilter)
                    {
                        items.Add((string)notebookDocumentFilter.Notebook.NotebookDocumentFilter!);
                    }
                }

                if (notebookDocumentFilter.HasCells)
                {
                    items.Add(string.Join(", ", notebookDocumentFilter.Cells!.Select(z => z.Language!)));
                }

                return $"[{string.Join(", ", items)}]";
            }

            public bool Equals(NotebookSelector? other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(Notebook, other.Notebook) && Equals(Cells, other.Cells);
            }

            public override bool Equals(object? obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((NotebookSelector)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ( ( Notebook != null ? Notebook.GetHashCode() : 0 ) * 397 ) ^ ( Cells != null ? Cells.GetHashCode() : 0 );
                }
            }

            public static bool operator ==(NotebookSelector? left, NotebookSelector? right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(NotebookSelector? left, NotebookSelector? right)
            {
                return !Equals(left, right);
            }
        }

        /// <summary>
        /// A collection of document filters used to identify valid documents
        /// </summary>
        [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
        public class NotebookDocumentSelector : ContainerBase<NotebookDocumentFilter>
        {
            public NotebookDocumentSelector() : this(Enumerable.Empty<NotebookDocumentFilter>())
            {
            }

            public NotebookDocumentSelector(IEnumerable<NotebookDocumentFilter> items) : base(items)
            {
            }

            public NotebookDocumentSelector(params NotebookDocumentFilter[] items) : base(items)
            {
            }

            public static implicit operator NotebookDocumentSelector(NotebookDocumentFilter[] items) => new NotebookDocumentSelector(items);

            public static implicit operator NotebookDocumentSelector(Collection<NotebookDocumentFilter> items) => new NotebookDocumentSelector(items);

            public static implicit operator NotebookDocumentSelector(List<NotebookDocumentFilter> items) => new NotebookDocumentSelector(items);

            public static implicit operator string(NotebookDocumentSelector? documentSelector) =>
                documentSelector is not null ? string.Join(", ", documentSelector.Select(x => (string)x)) : string.Empty;

            public bool IsMatch(NotebookDocumentAttributes attributes) => this.Any(z => z.IsMatch(attributes));

            public override string ToString() => this;

            public static NotebookDocumentSelector ForPattern(params string[] wildcards) =>
                new NotebookDocumentSelector(wildcards.Select(NotebookDocumentFilter.ForPattern));

            public static NotebookDocumentSelector ForNotebookType(params string[] notebookTypes) =>
                new NotebookDocumentSelector(notebookTypes.Select(NotebookDocumentFilter.ForNotebookType));

            public static NotebookDocumentSelector ForScheme(params string[] schemes) =>
                new NotebookDocumentSelector(schemes.Select(NotebookDocumentFilter.ForScheme));

            private string DebuggerDisplay => this;
        }


        /// <summary>
        /// A notebook cell kind.
        ///
        /// @since 3.17.0
        /// </summary>
        [JsonConverter(typeof(NumberEnumConverter))]
        public enum NotebookCellKind
        {
            /// <summary>
            /// A markup-cell is formatted source that is used for display.
            /// </summary>
            Markup = 1,

            /// <summary>
            /// A code-cell is source code.
            /// </summary>
            Code = 2
        }

        public interface INotebookDocumentRegistrationOptions : IRegistrationOptions
        {
            /// <summary>
            /// The notebooks to be synced
            /// </summary>
            Container<NotebookSelector> NotebookSelector { get; set; }
        }

        /// <summary>
        /// Options specific to a notebook plus its cells
        /// to be synced to the server.
        ///
        /// If a selector provider a notebook document
        /// filter but no cell selector all cells of a
        /// matching notebook document will be synced.
        ///
        /// If a selector provides no notebook document
        /// filter but only a cell selector all notebook
        /// document that contain at least one matching
        /// cell will be synced.
        ///
        /// @since 3.17.0
        /// </summary>
        [GenerateRegistrationOptions(nameof(ServerCapabilities.NotebookDocumentSync))]
        [RegistrationOptionsConverter(typeof(Converter))]
        public partial class NotebookDocumentSyncOptions : INotebookDocumentRegistrationOptions
        {
            /// <summary>
            /// The notebook to be synced. If a string
            /// value is provided it matches against the
            /// notebook type. '*' matches every notebook.
            /// </summary>
            public Container<NotebookSelector> NotebookSelector { get; set; }

            /// <summary>
            /// Whether save notification should be forwarded to
            /// the server. Will only be honored if mode === `notebook`.
            /// </summary>
            public bool? Save { get; set; }

            private class Converter : RegistrationOptionsConverterBase<NotebookDocumentSyncOptions, StaticOptions>
            {
                private readonly IHandlersManager _handlersManager;

                public Converter(IHandlersManager handlersManager)
                {
                    _handlersManager = handlersManager;
                }

                public override StaticOptions Convert(NotebookDocumentSyncOptions source)
                {
                    return new()
                    {
                        NotebookSelector = source.NotebookSelector,
                        Save = source.Save,
                    };
                }
            }
        }
    }

    namespace Client.Capabilities
    {
        /// <summary>
        /// Notebook specific client capabilities.
        ///
        /// @since 3.17.0
        /// </summary>
        public partial class NotebookDocumentSyncClientCapabilities : DynamicCapability
        {
            /// <summary>
            /// The client supports sending execution summary data per cell.
            /// </summary>
            public bool? ExecutionSummarySupport { get; set; }
        }
    }

    namespace Document
    {
        public interface INotebookDocumentSyncHandler : IDidChangeNotebookDocumentHandler, IDidOpenNotebookDocumentHandler,
                                                        IDidCloseNotebookDocumentHandler, IDidSaveNotebookDocumentHandler, INotebookDocumentIdentifier
        {
        }

        public abstract class NotebookDocumentSyncHandlerBase : INotebookDocumentSyncHandler
        {
            public abstract NotebookDocumentAttributes GetNotebookDocumentAttributes(DocumentUri uri);
            public abstract Task<Unit> Handle(DidOpenNotebookDocumentParams request, CancellationToken cancellationToken);
            public abstract Task<Unit> Handle(DidChangeNotebookDocumentParams request, CancellationToken cancellationToken);
            public abstract Task<Unit> Handle(DidSaveNotebookDocumentParams request, CancellationToken cancellationToken);
            public abstract Task<Unit> Handle(DidCloseNotebookDocumentParams request, CancellationToken cancellationToken);

            protected NotebookDocumentSyncOptions RegistrationOptions { get; private set; }

            protected ClientCapabilities ClientCapabilities { get; private set; }

            protected NotebookDocumentSyncClientCapabilities Capability { get; private set; } = default!;

            protected abstract NotebookDocumentSyncOptions CreateRegistrationOptions(
                NotebookDocumentSyncClientCapabilities capability, ClientCapabilities clientCapabilities
            );

            private NotebookDocumentSyncOptions AssignRegistrationOptions(
                NotebookDocumentSyncClientCapabilities capability, ClientCapabilities clientCapabilities
            )
            {
                Capability = capability;
                if (RegistrationOptions is { }) return RegistrationOptions;
                ClientCapabilities = clientCapabilities;
                return RegistrationOptions = CreateRegistrationOptions(capability, clientCapabilities);
            }

            NotebookDocumentSyncOptions IRegistration<NotebookDocumentSyncOptions, NotebookDocumentSyncClientCapabilities>.GetRegistrationOptions(
                NotebookDocumentSyncClientCapabilities capability, ClientCapabilities clientCapabilities
            )
            {
                return RegistrationOptions ?? AssignRegistrationOptions(capability, clientCapabilities);
            }
        }


        public static class NotebookDocumentSyncExtensions
        {
            public static ILanguageServerRegistry OnNotebookDocumentSync(
                this ILanguageServerRegistry registry,
                Func<DocumentUri, NotebookDocumentAttributes> getNotebookDocumentAttributes,
                Func<DidOpenNotebookDocumentParams, NotebookDocumentSyncClientCapabilities, CancellationToken, Task> onOpenHandler,
                Func<DidCloseNotebookDocumentParams, NotebookDocumentSyncClientCapabilities, CancellationToken, Task> onCloseHandler,
                Func<DidChangeNotebookDocumentParams, NotebookDocumentSyncClientCapabilities, CancellationToken, Task> onChangeHandler,
                Func<DidSaveNotebookDocumentParams, NotebookDocumentSyncClientCapabilities, CancellationToken, Task> onSaveHandler,
                RegistrationOptionsDelegate<NotebookDocumentSyncOptions, NotebookDocumentSyncClientCapabilities>? registrationOptions
            )
            {
                return registry.AddHandlers(
                    new DelegatingHandler(
                        getNotebookDocumentAttributes,
                        onOpenHandler,
                        onCloseHandler,
                        onChangeHandler,
                        onSaveHandler,
                        RegistrationAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(registrationOptions)
                    )
                );
            }

            public static ILanguageServerRegistry OnNotebookDocumentSync(
                this ILanguageServerRegistry registry,
                Func<DocumentUri, NotebookDocumentAttributes> getNotebookDocumentAttributes,
                Func<DidOpenNotebookDocumentParams, NotebookDocumentSyncClientCapabilities, CancellationToken, Task> onOpenHandler,
                Func<DidCloseNotebookDocumentParams, NotebookDocumentSyncClientCapabilities, CancellationToken, Task> onCloseHandler,
                Func<DidChangeNotebookDocumentParams, NotebookDocumentSyncClientCapabilities, CancellationToken, Task> onChangeHandler,
                Func<DidSaveNotebookDocumentParams, NotebookDocumentSyncClientCapabilities, CancellationToken, Task> onSaveHandler,
                NotebookDocumentSyncOptions? registrationOptions
            )
            {
                return registry.AddHandlers(
                    new DelegatingHandler(
                        getNotebookDocumentAttributes,
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onOpenHandler),
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onCloseHandler),
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onChangeHandler),
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onSaveHandler),
                        RegistrationAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(registrationOptions)
                    )
                );
            }

            public static ILanguageServerRegistry OnNotebookDocumentSync(
                this ILanguageServerRegistry registry,
                Func<DocumentUri, NotebookDocumentAttributes> getNotebookDocumentAttributes,
                Action<DidOpenNotebookDocumentParams, NotebookDocumentSyncClientCapabilities, CancellationToken> onOpenHandler,
                Action<DidCloseNotebookDocumentParams, NotebookDocumentSyncClientCapabilities, CancellationToken> onCloseHandler,
                Action<DidChangeNotebookDocumentParams, NotebookDocumentSyncClientCapabilities, CancellationToken> onChangeHandler,
                Action<DidSaveNotebookDocumentParams, NotebookDocumentSyncClientCapabilities, CancellationToken> onSaveHandler,
                RegistrationOptionsDelegate<NotebookDocumentSyncOptions, NotebookDocumentSyncClientCapabilities>? registrationOptions
            )
            {
                return registry.AddHandlers(
                    new DelegatingHandler(
                        getNotebookDocumentAttributes,
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onOpenHandler),
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onCloseHandler),
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onChangeHandler),
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onSaveHandler),
                        RegistrationAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(registrationOptions)
                    )
                );
            }

            public static ILanguageServerRegistry OnNotebookDocumentSync(
                this ILanguageServerRegistry registry,
                Func<DocumentUri, NotebookDocumentAttributes> getNotebookDocumentAttributes,
                Action<DidOpenNotebookDocumentParams, NotebookDocumentSyncClientCapabilities, CancellationToken> onOpenHandler,
                Action<DidCloseNotebookDocumentParams, NotebookDocumentSyncClientCapabilities, CancellationToken> onCloseHandler,
                Action<DidChangeNotebookDocumentParams, NotebookDocumentSyncClientCapabilities, CancellationToken> onChangeHandler,
                Action<DidSaveNotebookDocumentParams, NotebookDocumentSyncClientCapabilities, CancellationToken> onSaveHandler,
                NotebookDocumentSyncOptions? registrationOptions
            )
            {
                return registry.AddHandlers(
                    new DelegatingHandler(
                        getNotebookDocumentAttributes,
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onOpenHandler),
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onCloseHandler),
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onChangeHandler),
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onSaveHandler),
                        RegistrationAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(registrationOptions)
                    )
                );
            }

            public static ILanguageServerRegistry OnNotebookDocumentSync(
                this ILanguageServerRegistry registry,
                Func<DocumentUri, NotebookDocumentAttributes> getNotebookDocumentAttributes,
                Action<DidOpenNotebookDocumentParams, NotebookDocumentSyncClientCapabilities> onOpenHandler,
                Action<DidCloseNotebookDocumentParams, NotebookDocumentSyncClientCapabilities> onCloseHandler,
                Action<DidChangeNotebookDocumentParams, NotebookDocumentSyncClientCapabilities> onChangeHandler,
                Action<DidSaveNotebookDocumentParams, NotebookDocumentSyncClientCapabilities> onSaveHandler,
                RegistrationOptionsDelegate<NotebookDocumentSyncOptions, NotebookDocumentSyncClientCapabilities>? registrationOptions
            )
            {
                return registry.AddHandlers(
                    new DelegatingHandler(
                        getNotebookDocumentAttributes,
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onOpenHandler),
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onCloseHandler),
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onChangeHandler),
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onSaveHandler),
                        RegistrationAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(registrationOptions)
                    )
                );
            }

            public static ILanguageServerRegistry OnNotebookDocumentSync(
                this ILanguageServerRegistry registry,
                Func<DocumentUri, NotebookDocumentAttributes> getNotebookDocumentAttributes,
                Action<DidOpenNotebookDocumentParams, NotebookDocumentSyncClientCapabilities> onOpenHandler,
                Action<DidCloseNotebookDocumentParams, NotebookDocumentSyncClientCapabilities> onCloseHandler,
                Action<DidChangeNotebookDocumentParams, NotebookDocumentSyncClientCapabilities> onChangeHandler,
                Action<DidSaveNotebookDocumentParams, NotebookDocumentSyncClientCapabilities> onSaveHandler,
                NotebookDocumentSyncOptions? registrationOptions
            )
            {
                return registry.AddHandlers(
                    new DelegatingHandler(
                        getNotebookDocumentAttributes,
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onOpenHandler),
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onCloseHandler),
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onChangeHandler),
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onSaveHandler),
                        RegistrationAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(registrationOptions)
                    )
                );
            }

            public static ILanguageServerRegistry OnNotebookDocumentSync(
                this ILanguageServerRegistry registry,
                Func<DocumentUri, NotebookDocumentAttributes> getNotebookDocumentAttributes,
                Func<DidOpenNotebookDocumentParams, CancellationToken, Task> onOpenHandler,
                Func<DidCloseNotebookDocumentParams, CancellationToken, Task> onCloseHandler,
                Func<DidChangeNotebookDocumentParams, CancellationToken, Task> onChangeHandler,
                Func<DidSaveNotebookDocumentParams, CancellationToken, Task> onSaveHandler,
                RegistrationOptionsDelegate<NotebookDocumentSyncOptions, NotebookDocumentSyncClientCapabilities>? registrationOptions
            )
            {
                return registry.AddHandlers(
                    new DelegatingHandler(
                        getNotebookDocumentAttributes,
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onOpenHandler),
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onCloseHandler),
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onChangeHandler),
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onSaveHandler),
                        RegistrationAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(registrationOptions)
                    )
                );
            }

            public static ILanguageServerRegistry OnNotebookDocumentSync(
                this ILanguageServerRegistry registry,
                Func<DocumentUri, NotebookDocumentAttributes> getNotebookDocumentAttributes,
                Func<DidOpenNotebookDocumentParams, CancellationToken, Task> onOpenHandler,
                Func<DidCloseNotebookDocumentParams, CancellationToken, Task> onCloseHandler,
                Func<DidChangeNotebookDocumentParams, CancellationToken, Task> onChangeHandler,
                Func<DidSaveNotebookDocumentParams, CancellationToken, Task> onSaveHandler,
                NotebookDocumentSyncOptions? registrationOptions
            )
            {
                return registry.AddHandlers(
                    new DelegatingHandler(
                        getNotebookDocumentAttributes,
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onOpenHandler),
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onCloseHandler),
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onChangeHandler),
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onSaveHandler),
                        RegistrationAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(registrationOptions)
                    )
                );
            }

            public static ILanguageServerRegistry OnNotebookDocumentSync(
                this ILanguageServerRegistry registry,
                Func<DocumentUri, NotebookDocumentAttributes> getNotebookDocumentAttributes,
                Action<DidOpenNotebookDocumentParams, CancellationToken> onOpenHandler,
                Action<DidCloseNotebookDocumentParams, CancellationToken> onCloseHandler,
                Action<DidChangeNotebookDocumentParams, CancellationToken> onChangeHandler,
                Action<DidSaveNotebookDocumentParams, CancellationToken> onSaveHandler,
                RegistrationOptionsDelegate<NotebookDocumentSyncOptions, NotebookDocumentSyncClientCapabilities>? registrationOptions
            )
            {
                return registry.AddHandlers(
                    new DelegatingHandler(
                        getNotebookDocumentAttributes,
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onOpenHandler),
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onCloseHandler),
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onChangeHandler),
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onSaveHandler),
                        RegistrationAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(registrationOptions)
                    )
                );
            }

            public static ILanguageServerRegistry OnNotebookDocumentSync(
                this ILanguageServerRegistry registry,
                Func<DocumentUri, NotebookDocumentAttributes> getNotebookDocumentAttributes,
                Action<DidOpenNotebookDocumentParams, CancellationToken> onOpenHandler,
                Action<DidCloseNotebookDocumentParams, CancellationToken> onCloseHandler,
                Action<DidChangeNotebookDocumentParams, CancellationToken> onChangeHandler,
                Action<DidSaveNotebookDocumentParams, CancellationToken> onSaveHandler,
                NotebookDocumentSyncOptions? registrationOptions
            )
            {
                return registry.AddHandlers(
                    new DelegatingHandler(
                        getNotebookDocumentAttributes,
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onOpenHandler),
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onCloseHandler),
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onChangeHandler),
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onSaveHandler),
                        RegistrationAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(registrationOptions)
                    )
                );
            }

            public static ILanguageServerRegistry OnNotebookDocumentSync(
                this ILanguageServerRegistry registry,
                Func<DocumentUri, NotebookDocumentAttributes> getNotebookDocumentAttributes,
                Func<DidOpenNotebookDocumentParams, Task> onOpenHandler,
                Func<DidCloseNotebookDocumentParams, Task> onCloseHandler,
                Func<DidChangeNotebookDocumentParams, Task> onChangeHandler,
                Func<DidSaveNotebookDocumentParams, Task> onSaveHandler,
                RegistrationOptionsDelegate<NotebookDocumentSyncOptions, NotebookDocumentSyncClientCapabilities>? registrationOptions
            )
            {
                return registry.AddHandlers(
                    new DelegatingHandler(
                        getNotebookDocumentAttributes,
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onOpenHandler),
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onCloseHandler),
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onChangeHandler),
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onSaveHandler),
                        RegistrationAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(registrationOptions)
                    )
                );
            }

            public static ILanguageServerRegistry OnNotebookDocumentSync(
                this ILanguageServerRegistry registry,
                Func<DocumentUri, NotebookDocumentAttributes> getNotebookDocumentAttributes,
                Func<DidOpenNotebookDocumentParams, Task> onOpenHandler,
                Func<DidCloseNotebookDocumentParams, Task> onCloseHandler,
                Func<DidChangeNotebookDocumentParams, Task> onChangeHandler,
                Func<DidSaveNotebookDocumentParams, Task> onSaveHandler,
                NotebookDocumentSyncOptions? registrationOptions
            )
            {
                return registry.AddHandlers(
                    new DelegatingHandler(
                        getNotebookDocumentAttributes,
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onOpenHandler),
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onCloseHandler),
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onChangeHandler),
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onSaveHandler),
                        RegistrationAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(registrationOptions)
                    )
                );
            }

            public static ILanguageServerRegistry OnNotebookDocumentSync(
                this ILanguageServerRegistry registry,
                Func<DocumentUri, NotebookDocumentAttributes> getNotebookDocumentAttributes,
                Action<DidOpenNotebookDocumentParams> onOpenHandler,
                Action<DidCloseNotebookDocumentParams> onCloseHandler,
                Action<DidChangeNotebookDocumentParams> onChangeHandler,
                Action<DidSaveNotebookDocumentParams> onSaveHandler,
                RegistrationOptionsDelegate<NotebookDocumentSyncOptions, NotebookDocumentSyncClientCapabilities>? registrationOptions
            )
            {
                return registry.AddHandlers(
                    new DelegatingHandler(
                        getNotebookDocumentAttributes,
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onOpenHandler),
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onCloseHandler),
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onChangeHandler),
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onSaveHandler),
                        RegistrationAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(registrationOptions)
                    )
                );
            }

            public static ILanguageServerRegistry OnNotebookDocumentSync(
                this ILanguageServerRegistry registry,
                Func<DocumentUri, NotebookDocumentAttributes> getNotebookDocumentAttributes,
                Action<DidOpenNotebookDocumentParams> onOpenHandler,
                Action<DidCloseNotebookDocumentParams> onCloseHandler,
                Action<DidChangeNotebookDocumentParams> onChangeHandler,
                Action<DidSaveNotebookDocumentParams> onSaveHandler,
                NotebookDocumentSyncOptions? registrationOptions
            )
            {
                return registry.AddHandlers(
                    new DelegatingHandler(
                        getNotebookDocumentAttributes,
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onOpenHandler),
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onCloseHandler),
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onChangeHandler),
                        HandlerAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(onSaveHandler),
                        RegistrationAdapter<NotebookDocumentSyncClientCapabilities>.Adapt(registrationOptions)
                    )
                );
            }

            private class DelegatingHandler : NotebookDocumentSyncHandlerBase
            {
                private readonly Func<DidOpenNotebookDocumentParams, NotebookDocumentSyncClientCapabilities, CancellationToken, Task> _onOpenHandler;
                private readonly Func<DidCloseNotebookDocumentParams, NotebookDocumentSyncClientCapabilities, CancellationToken, Task> _onCloseHandler;
                private readonly Func<DidChangeNotebookDocumentParams, NotebookDocumentSyncClientCapabilities, CancellationToken, Task> _onChangeHandler;
                private readonly Func<DidSaveNotebookDocumentParams, NotebookDocumentSyncClientCapabilities, CancellationToken, Task> _onSaveHandler;

                private readonly RegistrationOptionsDelegate<NotebookDocumentSyncOptions, NotebookDocumentSyncClientCapabilities>
                    _registrationOptionsFactory;

                private readonly Func<DocumentUri, NotebookDocumentAttributes> _getNotebookDocumentAttributes;

                public DelegatingHandler(
                    Func<DocumentUri, NotebookDocumentAttributes> getNotebookDocumentAttributes,
                    Func<DidOpenNotebookDocumentParams, NotebookDocumentSyncClientCapabilities, CancellationToken, Task> onOpenHandler,
                    Func<DidCloseNotebookDocumentParams, NotebookDocumentSyncClientCapabilities, CancellationToken, Task> onCloseHandler,
                    Func<DidChangeNotebookDocumentParams, NotebookDocumentSyncClientCapabilities, CancellationToken, Task> onChangeHandler,
                    Func<DidSaveNotebookDocumentParams, NotebookDocumentSyncClientCapabilities, CancellationToken, Task> onSaveHandler,
                    RegistrationOptionsDelegate<NotebookDocumentSyncOptions, NotebookDocumentSyncClientCapabilities> registrationOptionsFactory
                )
                {
                    _onOpenHandler = onOpenHandler;
                    _onSaveHandler = onSaveHandler;
                    _registrationOptionsFactory = registrationOptionsFactory;
                    _onChangeHandler = onChangeHandler;
                    _onCloseHandler = onCloseHandler;
                    _getNotebookDocumentAttributes = getNotebookDocumentAttributes;
                }

                public override async Task<Unit> Handle(DidOpenNotebookDocumentParams request, CancellationToken cancellationToken)
                {
                    await _onOpenHandler.Invoke(request, Capability, cancellationToken).ConfigureAwait(false);
                    return Unit.Value;
                }

                public override async Task<Unit> Handle(DidChangeNotebookDocumentParams request, CancellationToken cancellationToken)
                {
                    await _onChangeHandler.Invoke(request, Capability, cancellationToken).ConfigureAwait(false);
                    return Unit.Value;
                }

                public override async Task<Unit> Handle(DidSaveNotebookDocumentParams request, CancellationToken cancellationToken)
                {
                    await _onSaveHandler.Invoke(request, Capability, cancellationToken).ConfigureAwait(false);
                    return Unit.Value;
                }

                public override async Task<Unit> Handle(DidCloseNotebookDocumentParams request, CancellationToken cancellationToken)
                {
                    await _onCloseHandler.Invoke(request, Capability, cancellationToken).ConfigureAwait(false);
                    return Unit.Value;
                }

                protected override NotebookDocumentSyncOptions CreateRegistrationOptions(
                    NotebookDocumentSyncClientCapabilities capability, ClientCapabilities clientCapabilities
                )
                {
                    return _registrationOptionsFactory(capability, clientCapabilities);
                }

                public override NotebookDocumentAttributes GetNotebookDocumentAttributes(DocumentUri uri)
                {
                    return _getNotebookDocumentAttributes.Invoke(uri);
                }
            }
        }
    }
}

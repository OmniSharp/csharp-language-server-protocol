using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{

    public struct DocumentSymbolInformationOrDocumentSymbol
    {
        private DocumentSymbol _documentSymbol;
        private DocumentSymbolInformation _command;
        public DocumentSymbolInformationOrDocumentSymbol(DocumentSymbol value)
        {
            _documentSymbol = value;
            _command = default;
        }
        public DocumentSymbolInformationOrDocumentSymbol(DocumentSymbolInformation value)
        {
            _documentSymbol = default;
            _command = value;
        }

        public bool IsDocumentSymbolInformation => this._command != null;
        public T DocumentSymbolInformation
        {
            get { return this._command; }
            set
            {
                this._command = value;
                this._documentSymbol = null;
            }
        }

        public bool IsDocumentSymbol => this._documentSymbol != null;
        public bool DocumentSymbol
        {
            get { return this._documentSymbol; }
            set
            {
                this._command = default;
                this._documentSymbol = value;
            }
        }
        public object RawValue
        {
            get
            {
                if (IsDocumentSymbolInformation) return DocumentSymbolInformation;
                if (IsDocumentSymbol) return DocumentSymbol;
                return default;
            }
        }

        public static implicit operator DocumentSymbolInformationOrDocumentSymbol(DocumentSymbolInformation value)
        {
            return new DocumentSymbolInformationOrDocumentSymbol(value);
        }

        public static implicit operator DocumentSymbolInformationOrDocumentSymbol(DocumentSymbol value)
        {
            return new DocumentSymbolInformationOrDocumentSymbol(value);
        }
    }

    public class DocumentSymbolInformation : ISymbolInformation
    {
        /// <summary>
        /// The name of this symbol.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The kind of this symbol.
        /// </summary>
        public SymbolKind Kind { get; set; }

        /// <summary>
        /// Indicates if this item is deprecated.
        /// </summary>
        [Optional]
        public bool Deprecated { get; set; }

        /// <summary>
        /// The location of this symbol.
        /// </summary>
        public Location Location { get; set; }

        /// <summary>
        /// The name of the symbol containing this symbol.
        /// </summary>
        [Optional]
        public string ContainerName { get; set; }
    }

    /// <summary>
    /// Represents programming constructs like variables, classes, interfaces etc. that appear in a document. Document symbols can be
    /// hierarchical and they have two ranges: one that encloses its definition and one that points to its most interesting range,
    /// e.g. the range of an identifier.
    /// </summary>
    public class DocumentSymbol
    {
        /// <summary>
        /// The name of this symbol.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// More detail for this symbol, e.g the signature of a function. If not provided the
        /// name is used.
        /// </summary>
        [Optional]
        public string Detail { get; set; }

        /// <summary>
        /// The kind of this symbol.
        /// </summary>
        public SymbolKind Kind { get; set; }

        /// <summary>
        /// Indicates if this symbol is deprecated.
        /// </summary>
        [Optional]
        public bool Deprecated { get; set; }

        /// <summary>
        /// The range enclosing this symbol not including leading/trailing whitespace but everything else
        /// like comments. This information is typically used to determine if the the clients cursor is
        /// inside the symbol to reveal in the symbol in the UI.
        /// </summary>
        public Range Range { get; set; }

        /// <summary>
        /// The range that should be selected and revealed when this symbol is being picked, e.g the name of a function.
        /// Must be contained by the the `range`.
        /// </summary>
        public Range SelectionRange { get; set; }

        /// <summary>
        /// Children of this symbol, e.g. properties of a class.
        /// </summary>
        [Optional]
        public Container<DocumentSymbol> Children { get; set; }
    }
}

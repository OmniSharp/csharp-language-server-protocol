using System.Diagnostics;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public struct SymbolInformationOrDocumentSymbol
    {
        private DocumentSymbol _documentSymbol;
        private SymbolInformation _symbolInformation;
        public SymbolInformationOrDocumentSymbol(DocumentSymbol documentSymbol)
        {
            _documentSymbol = documentSymbol;
            _symbolInformation = default;
        }
        public SymbolInformationOrDocumentSymbol(SymbolInformation symbolInformation)
        {
            _documentSymbol = default;
            _symbolInformation = symbolInformation;
        }

        public bool IsDocumentSymbolInformation => _symbolInformation != null;
        public SymbolInformation SymbolInformation => _symbolInformation;

        public bool IsDocumentSymbol => _documentSymbol != null;
        public DocumentSymbol DocumentSymbol => _documentSymbol;

        public static SymbolInformationOrDocumentSymbol Create(SymbolInformation value)
        {
            return value;
        }

        public static SymbolInformationOrDocumentSymbol Create(DocumentSymbol value)
        {
            return value;
        }

        public static implicit operator SymbolInformationOrDocumentSymbol(SymbolInformation value)
        {
            return new SymbolInformationOrDocumentSymbol(value);
        }

        public static implicit operator SymbolInformationOrDocumentSymbol(DocumentSymbol value)
        {
            return new SymbolInformationOrDocumentSymbol(value);
        }

        private string DebuggerDisplay => IsDocumentSymbol ? DocumentSymbol.ToString() : IsDocumentSymbolInformation ? SymbolInformation.ToString() : string.Empty;
        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
    }
}

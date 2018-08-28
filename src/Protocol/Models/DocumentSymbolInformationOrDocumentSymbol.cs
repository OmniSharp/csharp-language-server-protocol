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
        public DocumentSymbolInformation DocumentSymbolInformation
        {
            get { return this._command; }
            set
            {
                this._command = value;
                this._documentSymbol = null;
            }
        }

        public bool IsDocumentSymbol => this._documentSymbol != null;
        public DocumentSymbol DocumentSymbol
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
}

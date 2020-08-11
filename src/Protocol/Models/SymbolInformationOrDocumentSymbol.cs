using System.Diagnostics;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [JsonConverter(typeof(SymbolInformationOrDocumentSymbolConverter))]
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public struct SymbolInformationOrDocumentSymbol
    {
        public SymbolInformationOrDocumentSymbol(DocumentSymbol documentSymbol)
        {
            DocumentSymbol = documentSymbol;
            SymbolInformation = default;
        }

        public SymbolInformationOrDocumentSymbol(SymbolInformation symbolInformation)
        {
            DocumentSymbol = default;
            SymbolInformation = symbolInformation;
        }

        public bool IsDocumentSymbolInformation => SymbolInformation != null;
        public SymbolInformation SymbolInformation { get; }

        public bool IsDocumentSymbol => DocumentSymbol != null;
        public DocumentSymbol DocumentSymbol { get; }

        public static SymbolInformationOrDocumentSymbol Create(SymbolInformation value) => value;

        public static SymbolInformationOrDocumentSymbol Create(DocumentSymbol value) => value;

        public static implicit operator SymbolInformationOrDocumentSymbol(SymbolInformation value) => new SymbolInformationOrDocumentSymbol(value);

        public static implicit operator SymbolInformationOrDocumentSymbol(DocumentSymbol value) => new SymbolInformationOrDocumentSymbol(value);

        private string DebuggerDisplay => IsDocumentSymbol ? DocumentSymbol.ToString() : IsDocumentSymbolInformation ? SymbolInformation.ToString() : string.Empty;

        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
    }
}

using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public static class DocumentSymbolExtensions
    {
        public static Task<DocumentSymbolInformationOrDocumentSymbolContainer> DocumentSymbol(this ILanguageClientDocument mediator, DocumentSymbolParams @params)
        {
            return mediator.SendRequest<DocumentSymbolParams, DocumentSymbolInformationOrDocumentSymbolContainer>(DocumentNames.DocumentSymbol, @params);
        }
    }
}

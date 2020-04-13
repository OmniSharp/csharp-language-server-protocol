using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public static class DocumentSymbolExtensions
    {
        public static Task<SymbolInformationOrDocumentSymbolContainer> DocumentSymbol(this ILanguageClientDocument mediator, DocumentSymbolParams @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}

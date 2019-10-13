using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public static class DocumentHighlightExtensions
    {
        public static Task<DocumentHighlightContainer> DocumentHighlight(this ILanguageClientDocument mediator, DocumentHighlightParams @params)
        {
            return mediator.SendRequest<DocumentHighlightParams, DocumentHighlightContainer>(DocumentNames.DocumentHighlight, @params);
        }
    }
}

using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public static class DocumentLinkResolveExtensions
    {
        public static Task<DocumentLink> DocumentLinkResolve(this ILanguageClientDocument mediator, DocumentLink @params)
        {
            return mediator.SendRequest<DocumentLink, DocumentLink>(DocumentNames.DocumentLinkResolve, @params);
        }
    }
}

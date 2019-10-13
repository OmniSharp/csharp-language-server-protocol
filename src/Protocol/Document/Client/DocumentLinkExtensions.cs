using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public static class DocumentLinkExtensions
    {
        public static Task<DocumentLinkContainer> DocumentLink(this ILanguageClientDocument mediator, DocumentLinkParams @params)
        {
            return mediator.SendRequest<DocumentLinkParams, DocumentLinkContainer>(DocumentNames.DocumentLink, @params);
        }
    }
}

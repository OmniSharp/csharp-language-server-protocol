using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public static class DocumentFormattingExtensions
    {
        public static Task<Container<TextEdit>> DocumentFormatting(this ILanguageClientDocument mediator, DocumentFormattingParams @params)
        {
            return mediator.SendRequest<DocumentFormattingParams, Container<TextEdit>>(DocumentNames.Formatting, @params);
        }
    }
}

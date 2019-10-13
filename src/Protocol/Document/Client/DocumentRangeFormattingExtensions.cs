using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public static class DocumentRangeFormattingExtensions
    {
        public static Task<Container<TextEdit>> DocumentRangeFormatting(this ILanguageClientDocument mediator, DocumentRangeFormattingParams @params)
        {
            return mediator.SendRequest<DocumentRangeFormattingParams, Container<TextEdit>>(DocumentNames.RangeFormatting, @params);
        }
    }
}

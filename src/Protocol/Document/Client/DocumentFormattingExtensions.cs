using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public static class DocumentFormattingExtensions
    {
        public static Task<TextEditContainer> DocumentFormatting(this ILanguageClientDocument mediator, DocumentFormattingParams @params)
        {
            return mediator.SendRequest<DocumentFormattingParams, TextEditContainer>(DocumentNames.Formatting, @params);
        }
    }
}

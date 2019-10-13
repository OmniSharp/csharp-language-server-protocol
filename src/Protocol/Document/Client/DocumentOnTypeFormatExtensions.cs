using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public static class DocumentOnTypeFormatExtensions
    {
        public static Task<TextEditContainer> DocumentOnTypeFormat(this ILanguageClientDocument mediator, DocumentOnTypeFormattingParams @params)
        {
            return mediator.SendRequest<DocumentOnTypeFormattingParams, TextEditContainer>(DocumentNames.OnTypeFormatting, @params);
        }
    }
}

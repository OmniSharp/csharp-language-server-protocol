using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public static class DocumentRangeFormattingExtensions
    {
        public static Task<TextEditContainer> DocumentRangeFormatting(this ILanguageClientDocument mediator, DocumentRangeFormattingParams @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params,  cancellationToken);
        }
    }
}

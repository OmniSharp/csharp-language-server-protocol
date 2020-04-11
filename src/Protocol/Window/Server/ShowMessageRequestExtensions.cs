using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public static class ShowMessageRequestExtensions
    {
        public static Task<MessageActionItem> ShowMessage(this ILanguageServerWindow mediator, ShowMessageRequestParams @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }

        public static Task<MessageActionItem> Show(this ILanguageServerWindow mediator, ShowMessageRequestParams @params, CancellationToken cancellationToken = default)
        {
            return mediator.ShowMessage(@params, cancellationToken);
        }

        public static Task<MessageActionItem> Request(this ILanguageServerWindow mediator, ShowMessageRequestParams @params, CancellationToken cancellationToken = default)
        {
            return mediator.ShowMessage(@params, cancellationToken);
        }
    }
}

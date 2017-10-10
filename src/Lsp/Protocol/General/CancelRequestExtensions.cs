using OmniSharp.Extensions.LanguageServer.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public static class CancelRequestExtensions
    {
        public static void CancelRequest(this ILanguageServer mediator, CancelParams @params)
        {
            mediator.SendNotification<CancelParams>("$/cancelRequest", @params);
        }

        public static void CancelRequest(this ILanguageServer mediator, string id)
        {
            mediator.SendNotification<CancelParams>("$/cancelRequest", new CancelParams() { Id = id });
        }

        public static void CancelRequest(this ILanguageServer mediator, long id)
        {
            mediator.SendNotification<CancelParams>("$/cancelRequest", new CancelParams() { Id = id });
        }
    }
}

using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public static class CancelRequestExtensions
    {
        public static void CancelRequest(this ILanguageServer mediator, CancelParams @params)
        {
            mediator.SendNotification(GeneralNames.CancelRequest, @params);
        }

        public static void CancelRequest(this ILanguageServer mediator, string id)
        {
            mediator.SendNotification(GeneralNames.CancelRequest, new CancelParams() { Id = id });
        }

        public static void CancelRequest(this ILanguageServer mediator, long id)
        {
            mediator.SendNotification(GeneralNames.CancelRequest, new CancelParams() { Id = id });
        }
        public static void CancelRequest(this ILanguageClient mediator, CancelParams @params)
        {
            mediator.SendNotification(GeneralNames.CancelRequest, @params);
        }

        public static void CancelRequest(this ILanguageClient mediator, string id)
        {
            mediator.SendNotification(GeneralNames.CancelRequest, new CancelParams() { Id = id });
        }

        public static void CancelRequest(this ILanguageClient mediator, long id)
        {
            mediator.SendNotification(GeneralNames.CancelRequest, new CancelParams() { Id = id });
        }
    }
}

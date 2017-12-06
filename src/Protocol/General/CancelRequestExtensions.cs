using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public static partial class GeneralNames
    {
        public const string CancelRequest = "$/cancelRequest";
    }

    public static class CancelRequestExtensions
    {
        public static void CancelRequest(this IResponseRouter mediator, CancelParams @params)
        {
            mediator.SendNotification(GeneralNames.CancelRequest, @params);
        }

        public static void CancelRequest(this IResponseRouter mediator, string id)
        {
            mediator.SendNotification(GeneralNames.CancelRequest, new CancelParams() { Id = id });
        }

        public static void CancelRequest(this IResponseRouter mediator, long id)
        {
            mediator.SendNotification(GeneralNames.CancelRequest, new CancelParams() { Id = id });
        }
    }
}

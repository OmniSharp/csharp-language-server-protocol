

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.JsonRpc
{
    public static class CancelRequestExtensions
    {
        public static void CancelRequest(this IResponseRouter mediator, CancelParams @params)
        {
            mediator.SendNotification(JsonRpcNames.CancelRequest, @params);
        }

        public static void CancelRequest(this IResponseRouter mediator, string id)
        {
            mediator.SendNotification(JsonRpcNames.CancelRequest, new CancelParams() { Id = id });
        }

        public static void CancelRequest(this IResponseRouter mediator, long id)
        {
            mediator.SendNotification(JsonRpcNames.CancelRequest, new CancelParams() { Id = id });
        }
    }
}

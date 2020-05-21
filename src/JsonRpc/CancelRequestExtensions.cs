



namespace OmniSharp.Extensions.JsonRpc
{
    public static class JsonRpcCancelRequestExtensions
    {
        public static void CancelRequest(this IResponseRouter mediator, CancelParams @params)
        {
            mediator.SendNotification(@params);
        }

        public static void CancelRequest(this IResponseRouter mediator, string id)
        {
            mediator.SendNotification(new CancelParams() { Id = id });
        }

        public static void CancelRequest(this IResponseRouter mediator, long id)
        {
            mediator.SendNotification(new CancelParams() { Id = id });
        }
    }
}

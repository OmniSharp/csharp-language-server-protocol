namespace OmniSharp.Extensions.JsonRpc.Server
{
    /// <summary>
    /// Request, Error, Notification or Response
    /// :)
    /// </summary>
    public struct Renor
    {
        internal Renor(Notification notification)
        {
            Notification = notification;
            Request = null;
            Error = null;
            Response = null;
        }

        internal Renor(Request request)
        {
            Notification = null;
            Request = request;
            Error = null;
            Response = null;
        }

        internal Renor(RpcError errorMessage)
        {
            Notification = null;
            Request = null;
            Error = errorMessage;
            Response = null;
        }

        internal Renor(ResponseBase response)
        {
            Notification = null;
            Request = null;
            Error = null;
            Response = response;
        }

        public bool IsNotification => Notification != null;
        public Notification Notification { get; }

        public bool IsRequest => Request != null;
        public Request Request { get; }

        public bool IsError => Error != null;
        public RpcError Error { get; }

        public bool IsResponse => Response != null;
        public ResponseBase Response { get; }

        public static implicit operator Renor(Notification notification)
        {
            return new Renor(notification);
        }

        public static implicit operator Renor(Request request)
        {
            return new Renor(request);
        }

        public static implicit operator Renor(RpcError error)
        {
            return new Renor(error);
        }

        public static implicit operator Renor(ServerResponse response)
        {
            return new Renor(response);
        }

        public static implicit operator Renor(ServerError response)
        {
            return new Renor(response);
        }
    }
}

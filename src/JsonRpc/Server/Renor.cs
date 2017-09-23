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

        internal Renor(Error errorMessage)
        {
            Notification = null;
            Request = null;
            Error = errorMessage;
            Response = null;
        }

        internal Renor(Response response)
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
        public Error Error { get; }

        public bool IsResponse => Response != null;
        public Response Response { get; }

        public static implicit operator Renor(Notification notification)
        {
            return new Renor(notification);
        }

        public static implicit operator Renor(Request request)
        {
            return new Renor(request);
        }

        public static implicit operator Renor(Error error)
        {
            return new Renor(error);
        }

        public static implicit operator Renor(Response response)
        {
            return new Renor(response);
        }
    }
}
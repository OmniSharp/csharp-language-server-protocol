namespace JsonRPC.Server
{
    public struct ErrorNotificationRequest
    {
        internal ErrorNotificationRequest(Notification notification)
        {
            Notification = notification;
            Request = null;
            Error = null;
        }

        internal ErrorNotificationRequest(Request request)
        {
            Notification = null;
            Request = request;
            Error = null;
        }

        internal ErrorNotificationRequest(Error errorMessage)
        {
            Notification = null;
            Request = null;
            Error = errorMessage;
        }

        public bool IsNotification => Notification != null;
        public Notification Notification { get; }

        public bool IsRequest => Request != null;
        public Request Request { get; }

        public bool IsError => Error != null;
        public Error Error { get; }
    }
}
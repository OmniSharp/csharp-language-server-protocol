namespace JsonRpc.Server
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

        public static implicit operator ErrorNotificationRequest(Notification notification)
        {
            return new ErrorNotificationRequest(notification);
        }

        public static implicit operator ErrorNotificationRequest(Request request)
        {
            return new ErrorNotificationRequest(request);
        }

        public static implicit operator ErrorNotificationRequest(Error error)
        {
            return new ErrorNotificationRequest(error);
        }
    }
}
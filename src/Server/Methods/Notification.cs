using System;

namespace OmniSharp.Extensions.LanguageServer.Server.Methods
{
    public class Notification
    {
        public Notification(string method)
        {
            Method = method;
        }

        public Notification(string method, Type paramsType) : this(method)
        {
            ParamsType = paramsType;
        }

        public string Method { get; }
        public Type ParamsType { get; }
    }

    public class Notification<TParams> : Notification
    {
        public Notification(string method) : base(method, typeof(TParams)) { }
    }

    public class DynamicNotification<TRegistration> : DynamicNotification
    {
        public DynamicNotification(string method) : base(method, typeof(TRegistration)) { }
    }

    public class DynamicNotification<TParams, TRegistration> : DynamicNotification
    {
        public DynamicNotification(string method) : base(method, typeof(TRegistration), typeof(TParams)) { }
    }
}
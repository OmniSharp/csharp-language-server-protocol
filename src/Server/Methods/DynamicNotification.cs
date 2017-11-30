using System;

namespace OmniSharp.Extensions.LanguageServer.Server.Methods
{
    public class DynamicNotification
    {
        public DynamicNotification(string method, Type registrationType)
        {
            Method = method;
            RegistrationType = registrationType;
        }

        public DynamicNotification(string method, Type registrationType, Type paramsType) : this(method, registrationType)
        {
            ParamsType = paramsType;
        }

        public string Method { get; }

        public Type ParamsType { get; }

        public Type RegistrationType { get; }
    }
}
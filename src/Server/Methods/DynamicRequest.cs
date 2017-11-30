using System;

namespace OmniSharp.Extensions.LanguageServer.Methods
{
    public class DynamicRequest
    {
        public DynamicRequest(string method, Type registrationType)
        {
            Method = method;
            RegistrationType = registrationType;
        }

        public DynamicRequest(string method, Type registrationType, Type paramsType) : this(method, registrationType)
        {
            ParamsType = paramsType;
        }

        public DynamicRequest(string method, Type registrationType, Type paramsType, Type responseType) : this(method, registrationType, paramsType)
        {
            ResponseType = responseType;
        }

        public DynamicRequest(string method, Type registrationType, Type paramsType, Type responseType, Type errorType) : this(method, registrationType, paramsType, responseType)
        {
            ErrorType = errorType;
        }

        public string Method { get; }
        public Type ParamsType { get; }
        public Type ResponseType { get; }
        public Type ErrorType { get; }

        public Type RegistrationType { get; }
    }
}
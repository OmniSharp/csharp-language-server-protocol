using System;

namespace Lsp.Methods
{
    public class Request
    {
        public Request(string method)
        {
            Method = method;
        }

        public Request(string method, Type paramsType) : this(method)
        {
            ParamsType = paramsType;
        }

        public Request(string method, Type paramsType, Type responseType) : this(method, paramsType)
        {
            ResponseType = responseType;
        }

        public Request(string method, Type paramsType, Type responseType, Type errorType) : this(method, paramsType, responseType)
        {
            ErrorType = errorType;
        }

        public string Method { get; }
        public Type ParamsType { get; }
        public Type ResponseType { get; }
        public Type ErrorType { get; }
    }
}
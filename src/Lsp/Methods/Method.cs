namespace Lsp.Methods
{
    public class Request<TParams> : Request
    {
        public Request(string method) : base(method, typeof(TParams)) { }
    }

    public class DynamicRequest<TParams, TRegistration> : DynamicRequest
    {
        public DynamicRequest(string method) : base(method, typeof(TRegistration), typeof(TParams)) { }
    }

    public class Request<TParams, TResponse> : Request
    {
        public Request(string method) : base(method, typeof(TParams), typeof(TResponse)) { }
    }

    public class DynamicRequest<TParams, TResponse, TRegistration> : DynamicRequest
    {
        public DynamicRequest(string method) : base(method, typeof(TRegistration), typeof(TParams), typeof(TResponse)) { }
    }

    public class Request<TParams, TResponse, TError> : Request
    {
        public Request(string method) : base(method, typeof(TParams), typeof(TResponse), typeof(TError)) { }
    }

    public class DynamicRequest<TParams, TResponse, TError, TRegistration> : DynamicRequest
    {
        public DynamicRequest(string method) : base(method, typeof(TRegistration), typeof(TParams), typeof(TResponse), typeof(TError)) { }
    }
}
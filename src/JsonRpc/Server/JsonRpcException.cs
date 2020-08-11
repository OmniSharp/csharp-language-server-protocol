namespace OmniSharp.Extensions.JsonRpc.Server
{
    public class JsonRpcException : RequestException
    {
        public JsonRpcException(int code, object requestId, string message, string data) : base(code, requestId, message) => Error = data;

        public string Error { get; }
    }
}

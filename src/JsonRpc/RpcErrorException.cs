namespace OmniSharp.Extensions.JsonRpc
{
    public class RpcErrorException : Exception
    {
        public RpcErrorException(int code, object error)
        {
            Error = error;
            Code = code;
        }

        public RpcErrorException(int code, object error, string message) : base(message)
        {
            Error = error;
            Code = code;
        }

        public object Error { get; }
        public int Code { get; }
    }
}

namespace OmniSharp.Extensions.JsonRpc.Server.Messages
{
    public class InternalError : RpcError
    {
        public InternalError() : this(null, null) { }
        public InternalError(object id, string method) : base(id, method, new ErrorMessage(ErrorCodes.InternalError, "Internal Error")) { }
        public InternalError(object id, string method, string message) : base(id, method, new ErrorMessage(ErrorCodes.InternalError, "Internal Error - " + message)) { }
    }
}

namespace OmniSharp.Extensions.JsonRpc.Server.Messages
{
    public class InternalError : RpcError
    {
        public InternalError() : this(null) { }
        public InternalError(object id) : base(id, new ErrorMessage(ErrorCodes.InternalError, "Internal Error")) { }
        public InternalError(object id, string message) : base(id, new ErrorMessage(ErrorCodes.InternalError, "Internal Error - " + message)) { }
    }
}

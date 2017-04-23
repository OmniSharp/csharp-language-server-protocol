namespace JsonRpc.Server.Messages
{
    public class InternalError : Error
    {
        public InternalError() : this(null) { }
        public InternalError(object id) : base(id, new ErrorMessage(-32602, "Internal Error")) { }
    }
}
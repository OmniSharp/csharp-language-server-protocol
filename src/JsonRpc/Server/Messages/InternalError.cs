namespace JsonRPC.Server
{
    public class InternalError : Error
    {
        internal InternalError() : this(null) { }
        internal InternalError(object id) : base(id, new ErrorMessage(-32602, "Internal Error")) { }
    }
}
namespace JsonRpc.Server.Messages
{
    public class InvalidParams : Error
    {
        internal InvalidParams() : this(null) { }
        internal InvalidParams(object id) : base(id, new ErrorMessage(-32602, "Invalid params")) { }
    }
}
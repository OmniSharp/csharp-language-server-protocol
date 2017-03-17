namespace JsonRpc.Server.Messages
{
    public class MethodNotFound : Error
    {
        internal MethodNotFound(object id) : base(id, new ErrorMessage(-32601, "Method not found")) { }
    }
}
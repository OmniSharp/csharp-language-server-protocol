namespace OmniSharp.Extensions.JsonRpc.Server.Messages
{
    public class MethodNotFound : Error
    {
        public MethodNotFound(object id) : base(id, new ErrorMessage(-32601, "Method not found")) { }
    }
}
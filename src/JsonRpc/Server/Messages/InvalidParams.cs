namespace OmniSharp.Extensions.JsonRpc.Server.Messages
{
    public class InvalidParams : RpcError
    {
        public InvalidParams() : this(null) { }
        public InvalidParams(object id) : base(id, new ErrorMessage(-32602, "Invalid params")) { }
    }
}

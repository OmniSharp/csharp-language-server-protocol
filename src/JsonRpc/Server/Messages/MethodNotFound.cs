namespace OmniSharp.Extensions.JsonRpc.Server.Messages
{
    public class MethodNotFound : RpcError
    {
        public MethodNotFound(object id, string method) : base(id, method, new ErrorMessage(-32601, $"Method not found - {method}")) { }
    }
}

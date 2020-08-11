namespace OmniSharp.Extensions.JsonRpc.Server.Messages
{
    public class InvalidParams : RpcError
    {
        public InvalidParams(string method) : this(null, method)
        {
        }

        public InvalidParams(object id, string method) : base(id, method, new ErrorMessage(-32602, "Invalid params"))
        {
        }
    }
}

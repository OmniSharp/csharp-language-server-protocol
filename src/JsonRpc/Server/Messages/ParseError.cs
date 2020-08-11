namespace OmniSharp.Extensions.JsonRpc.Server.Messages
{
    public class ParseError : RpcError
    {
        public ParseError(string method) : this(null, method)
        {
        }

        public ParseError(object id, string method) : base(id, method, new ErrorMessage(-32700, "Parse Error"))
        {
        }
    }
}

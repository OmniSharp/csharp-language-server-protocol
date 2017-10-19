namespace OmniSharp.Extensions.JsonRpc.Server.Messages
{
    public class ParseError : RpcError
    {
        public ParseError() : this(null) { }
        public ParseError(object id) : base(id, new ErrorMessage(-32700, "Parse Error")) { }
    }
}

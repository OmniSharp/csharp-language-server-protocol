namespace JsonRpc.Server.Messages
{
    public class ParseError : Error
    {
        public ParseError() : this(null) { }
        public ParseError(object id) : base(id, new ErrorMessage(-32700, "Parse Error")) { }
    }
}
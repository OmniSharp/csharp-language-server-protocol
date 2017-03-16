namespace JsonRPC.Server
{
    public class ParseError : Error
    {
        internal ParseError() : this(null) { }
        internal ParseError(object id) : base(id, new ErrorMessage(-32700, "Parse Error")) { }
    }
}
namespace JsonRpc.Server.Messages
{
    public class InvalidRequest : Error
    {

        internal InvalidRequest() : base(null, new ErrorMessage(-32600, $"Invalid Request")) { }
        internal InvalidRequest(object id) : base(id, new ErrorMessage(-32600, $"Invalid Request")) { }
        internal InvalidRequest(string message) : base(null, new ErrorMessage(-32600, $"Invalid Request - {message}")) { }
        internal InvalidRequest(object id, string message) : base(id, new ErrorMessage(-32600, $"Invalid Request - {message}")) { }
    }
}
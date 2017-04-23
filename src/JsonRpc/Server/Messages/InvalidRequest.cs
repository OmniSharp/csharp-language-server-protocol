namespace JsonRpc.Server.Messages
{
    public class InvalidRequest : Error
    {

        public InvalidRequest() : base(null, new ErrorMessage(-32600, $"Invalid Request")) { }
        public InvalidRequest(object id) : base(id, new ErrorMessage(-32600, $"Invalid Request")) { }
        public InvalidRequest(string message) : base(null, new ErrorMessage(-32600, $"Invalid Request - {message}")) { }
        public InvalidRequest(object id, string message) : base(id, new ErrorMessage(-32600, $"Invalid Request - {message}")) { }
    }
}
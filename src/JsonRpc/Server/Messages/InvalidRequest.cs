namespace OmniSharp.Extensions.JsonRpc.Server.Messages
{
    public class InvalidRequest : RpcError
    {
        public InvalidRequest(string? method) : base(null, method, new ErrorMessage(-32600, "Invalid Request"))
        {
        }

        public InvalidRequest(string? method, string message) : base(null, method, new ErrorMessage(-32600, $"Invalid Request - {message}"))
        {
        }

        public InvalidRequest(object? id, string? method, string message) : base(id, method, new ErrorMessage(-32600, $"Invalid Request - {message}"))
        {
        }
    }
}

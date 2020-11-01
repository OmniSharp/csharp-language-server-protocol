namespace OmniSharp.Extensions.JsonRpc.Server
{
    public class ServerError : ResponseBase
    {
        public ServerError(ServerErrorResult result) : this(null, result) => Error = result;

        public ServerError(object? id, ServerErrorResult result) : base(id) => Error = result;

        public ServerErrorResult Error { get; }
    }
}

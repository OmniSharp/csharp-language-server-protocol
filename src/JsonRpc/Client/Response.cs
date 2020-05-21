using ServerRequest = OmniSharp.Extensions.JsonRpc.Server.Request;

namespace OmniSharp.Extensions.JsonRpc.Client
{
    public class Response
    {
        public Response(object id, ServerRequest request)
        {
            Id = id;
            Request = request;
        }

        public Response(object id, object result, ServerRequest request)
        {
            Id = id;
            Result = result;
            Request = request;
        }

        public object Id { get; set; }

        public object Result { get; set; }
        public ServerRequest Request { get; }
    }
}

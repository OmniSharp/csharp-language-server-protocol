using ServerRequest = OmniSharp.Extensions.JsonRpc.Server.Request;

namespace OmniSharp.Extensions.JsonRpc.Client
{
    public class OutgoingResponse
    {
        public OutgoingResponse(object id, ServerRequest request)
        {
            Id = id;
            Request = request;
        }

        public OutgoingResponse(object id, object result, ServerRequest request)
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

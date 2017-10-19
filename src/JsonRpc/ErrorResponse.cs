using OmniSharp.Extensions.JsonRpc.Client;

namespace OmniSharp.Extensions.JsonRpc
{
    public struct ErrorResponse
    {
        public ErrorResponse(RpcError error)
        {
            Response = null;
            Error = error;
        }

        public ErrorResponse(Response response)
        {
            Response = response;
            Error = null;
        }

        public bool IsResponse => Response != null;
        public Response Response { get; }

        public bool IsError => Error != null;
        public RpcError Error { get; }
        public object Value => IsResponse ? (object)Response : IsError ? Error : null;

        public static implicit operator ErrorResponse(Response response)
        {
            return new ErrorResponse(response);
        }

        public static implicit operator ErrorResponse(RpcError error)
        {
            return new ErrorResponse(error);
        }
    }
}

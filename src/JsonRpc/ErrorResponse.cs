using JsonRpc.Client;

namespace JsonRpc
{
    public struct ErrorResponse
    {
        public ErrorResponse(Error error)
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
        public Error Error { get; }

        public static implicit operator ErrorResponse(Response response)
        {
            return new ErrorResponse(response);
        }

        public static implicit operator ErrorResponse(Error error)
        {
            return new ErrorResponse(error);
        }
    }
}
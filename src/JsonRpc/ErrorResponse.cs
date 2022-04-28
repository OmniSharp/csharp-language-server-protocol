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

        public ErrorResponse(OutgoingResponse outgoingResponse)
        {
            Response = outgoingResponse;
            Error = null;
        }

        public bool IsResponse => Response != null;
        public OutgoingResponse? Response { get; }

        public bool IsError => Error != null;
        public RpcError? Error { get; }
        public object? Value => IsResponse ? Response : IsError ? Error : null;

        public static implicit operator ErrorResponse(OutgoingResponse outgoingResponse)
        {
            return new ErrorResponse(outgoingResponse);
        }

        public static implicit operator ErrorResponse(RpcError error)
        {
            return new ErrorResponse(error);
        }
    }
}

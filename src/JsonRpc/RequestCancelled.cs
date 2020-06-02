using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.JsonRpc.Server.Messages;

namespace OmniSharp.Extensions.JsonRpc
{
    public class RequestCancelled : RpcError
    {
        internal RequestCancelled() : base(null, new ErrorMessage(ErrorCodes.RequestCancelled, "Request Cancelled")) { }
        internal RequestCancelled(object id) : base(id, new ErrorMessage(ErrorCodes.RequestCancelled, "Request Cancelled")) { }
    }
}

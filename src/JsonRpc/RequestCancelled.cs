using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.JsonRpc.Server.Messages;

namespace OmniSharp.Extensions.JsonRpc
{
    public class RequestCancelled : RpcError
    {
        internal RequestCancelled(string method) : base(null, method, new ErrorMessage(ErrorCodes.RequestCancelled, "Request Cancelled"))
        {
        }

        internal RequestCancelled(object id, string method) : base(id, method, new ErrorMessage(ErrorCodes.RequestCancelled, "Request Cancelled"))
        {
        }
    }
}

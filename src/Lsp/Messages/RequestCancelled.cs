using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Server.Messages;

namespace OmniSharp.Extensions.LanguageServer.Messages
{
    public class RequestCancelled : RpcError
    {
        internal RequestCancelled() : base(null, new ErrorMessage(-32800, "Request Cancelled")) { }
    }
}

using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Server.Messages;

namespace OmniSharp.Extensions.LanguageServerProtocol.Messages
{
    public class RequestCancelled : Error
    {
        internal RequestCancelled() : base(null, new ErrorMessage(-32800, "Request Cancelled")) { }
    }
}
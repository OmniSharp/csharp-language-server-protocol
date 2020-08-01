using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Server.Messages;

namespace OmniSharp.Extensions.LanguageServer.Server.Messages
{
    public class UnknownErrorCode : RpcError
    {
        internal UnknownErrorCode(string method) : base(null, method, new ErrorMessage(-32602, "Unknown Error Code")) { }
    }
}

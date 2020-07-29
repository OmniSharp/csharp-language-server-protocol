using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Server.Messages;

namespace OmniSharp.Extensions.LanguageServer.Server.Messages
{
    public class ServerNotInitialized : RpcError
    {
        internal ServerNotInitialized(string method) : base(null, method, new ErrorMessage(-32002, "Server Not Initialized")) { }
    }
}
